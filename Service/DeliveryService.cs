using Core.DTOs;
using Core.DTOs.DeliveryDTOs;
using Core.DTOs.DeliveryPersonDTOs;
using Core.Entities;
using Core.Enums;
using Core.Interfaces;
using Core.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Service.Utilities;
using System.Transactions;
using TechpertsSolutions.Core.Entities;

namespace Service
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IRepository<Order> _orderRepo;
        private readonly IRepository<Delivery> _deliveryRepo;
        private readonly IRepository<DeliveryOffer> _deliveryOfferRepo;
        private readonly IRepository<DeliveryPerson> _deliveryPersonRepo;
        private readonly IRepository<TechCompany> _techCompanyRepo;
        private readonly IRepository<DeliveryCluster> _deliveryClusterRepo;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILocationService _locationService;
        private readonly IDeliveryPersonService _deliveryPersonService;
        private readonly INotificationService _notificationService;
        private readonly DeliveryAssignmentSettings _settings;
        private readonly ILogger<DeliveryService> _logger;

        public DeliveryService(
            IRepository<Order> orderRepo,
            IRepository<Delivery> deliveryRepo,
            IRepository<DeliveryOffer> deliveryOfferRepo,
            IRepository<DeliveryPerson> deliveryPersonRepo,
            IRepository<TechCompany> techCompanyRepo,
            IRepository<DeliveryCluster> deliveryClusterRepo,
            IServiceProvider serviceProvider,
            ILocationService locationService,
            IDeliveryPersonService deliveryPersonService,
            INotificationService notificationService,
            IOptions<DeliveryAssignmentSettings> settings,
            ILogger<DeliveryService> logger
        )
        {
            _orderRepo = orderRepo ?? throw new ArgumentNullException(nameof(orderRepo));
            _deliveryRepo = deliveryRepo ?? throw new ArgumentNullException(nameof(deliveryRepo));
            _deliveryOfferRepo =
                deliveryOfferRepo ?? throw new ArgumentNullException(nameof(deliveryOfferRepo));
            _deliveryPersonRepo =
                deliveryPersonRepo ?? throw new ArgumentNullException(nameof(deliveryPersonRepo));
            _techCompanyRepo =
                techCompanyRepo ?? throw new ArgumentNullException(nameof(techCompanyRepo));
            _deliveryClusterRepo =
                deliveryClusterRepo ?? throw new ArgumentNullException(nameof(deliveryClusterRepo));
            _serviceProvider =
                serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _locationService =
                locationService ?? throw new ArgumentNullException(nameof(locationService));
            _deliveryPersonService =
                deliveryPersonService
                ?? throw new ArgumentNullException(nameof(deliveryPersonService));
            _notificationService =
                notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private IDeliveryClusterService _clusterService =>
            _serviceProvider.GetRequiredService<IDeliveryClusterService>();

        public async Task<GeneralResponse<DeliveryReadDTO>> CreateAsync(DeliveryCreateDTO dto)
        {
            _logger.LogInformation("CreateAsync: Starting delivery creation for OrderId {OrderId}", dto?.OrderId);

            if (dto == null
                || string.IsNullOrWhiteSpace(dto.OrderId)
                || dto.CustomerLatitude < -90 || dto.CustomerLatitude > 90
                || dto.CustomerLongitude < -180 || dto.CustomerLongitude > 180)
            {
                return new GeneralResponse<DeliveryReadDTO>
                {
                    Success = false,
                    Message = "Delivery data, OrderId, and customer location are required."
                };
            }

            try
            {
                var order = await _orderRepo.GetFirstOrDefaultAsync(
                   o => o.Id == dto.OrderId,
                   query => query.Include(o => o.OrderItems)
                                 .ThenInclude(oi => oi.Product)
                                 .ThenInclude(p => p.TechCompany)
                                     .ThenInclude(tc => tc.User)
               );

                if (order == null)
                    return new GeneralResponse<DeliveryReadDTO>
                    {
                        Success = false,
                        Message = "Order not found."
                    };

                var distinctCompanies = order.OrderItems
                    .Select(i => i.Product?.TechCompany)
                    .Where(c => c != null)
                    .Distinct()
                    .ToList();

                bool isComplex = distinctCompanies.Count > 1;

                var delivery = DeliveryMapper.ToEntity(dto);
                delivery.OrderId = order.Id;
                delivery.CustomerId = order.CustomerId;
                delivery.DropoffLatitude = dto.CustomerLatitude;
                delivery.DropoffLongitude = dto.CustomerLongitude;

                await _deliveryRepo.AddAsync(delivery);
                await _deliveryRepo.SaveChangesAsync();

                var createdClusters = new List<DeliveryClusterDTO>();

                foreach (var company in distinctCompanies)
                {
                    var clusterDto = new DeliveryClusterCreateDTO
                    {
                        DeliveryId = delivery.Id,
                        TechCompanyId = company.Id,
                        TechCompanyName = company.User?.FullName ?? "Unknown",
                        DropoffLatitude = dto.CustomerLatitude,
                        DropoffLongitude = dto.CustomerLongitude,
                    };

                    var clusterResult = await _clusterService.CreateClusterAsync(delivery.Id ,clusterDto);
                    if (!clusterResult.Success || clusterResult.Data == null)
                        throw new InvalidOperationException($"Cluster creation failed: {clusterResult.Message}");

                    createdClusters.Add(clusterResult.Data);
                }


                foreach (var cluster in createdClusters)
                {
                    var company = distinctCompanies.First(c => c.Id == cluster.TechCompanyId);
                    double pickupLat = company.User.Latitude ?? dto.CustomerLatitude;
                    double pickupLng = company.User.Longitude ?? dto.CustomerLongitude;
                    await AutoAssignDriverAsync(delivery, cluster.Id, pickupLat, pickupLng);
                }

                var finalDto = DeliveryMapper.ToReadDTO(delivery, createdClusters);

                return new GeneralResponse<DeliveryReadDTO>
                {
                    Success = true,
                    Message = "Delivery created successfully.",
                    Data = finalDto
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<DeliveryReadDTO>
                {
                    Success = false,
                    Message = $"Failed to create delivery: {ex.Message}"
                };
            }
        }

        public async Task AutoAssignDriverAsync(Delivery delivery, string clusterId, double? overrideLat = null, double? overrideLon = null)
        {
            _logger.LogInformation("AutoAssignDriverAsync: Starting for Delivery {DeliveryId}, Cluster {ClusterId}", delivery?.Id, clusterId);

            if (delivery == null || string.IsNullOrWhiteSpace(clusterId))
                throw new ArgumentNullException("Delivery and cluster ID are required.");

            try
            {
                DeliveryClusterDTO clusterDto = null;
                if (!string.IsNullOrWhiteSpace(clusterId) && clusterId != "default-cluster-id")
                {
                    var clusterResult = await _clusterService.GetByIdAsync(clusterId);
                    if (!clusterResult.Success || clusterResult.Data == null)
                        throw new InvalidOperationException("Cluster not found.");
                    clusterDto = clusterResult.Data;

                    _logger.LogInformation("AutoAssignDriverAsync: Using real cluster {ClusterId}", clusterId);
                }
                else
                {
                    clusterDto = new DeliveryClusterDTO
                    {
                        Id = clusterId,
                        DropoffLatitude = overrideLat ?? delivery.DropoffLatitude,
                        DropoffLongitude = overrideLon ?? delivery.DropoffLongitude,
                        DeliveryId = delivery.Id,
                        TechCompanyId = null
                    };
                    _logger.LogInformation("AutoAssignDriverAsync: Using temporary cluster for Delivery {DeliveryId}", delivery.Id);
                }

                double locationLat, locationLon;
                if (overrideLat.HasValue && overrideLon.HasValue)
                {
                    locationLat = overrideLat.Value;
                    locationLon = overrideLon.Value;
                }
                else if (!string.IsNullOrWhiteSpace(clusterDto.TechCompanyId))
                {
                    var techCompany = await _techCompanyRepo.GetByIdWithIncludesAsync(clusterDto.TechCompanyId);
                    if (techCompany == null || !techCompany.User.Latitude.HasValue || !techCompany.User.Longitude.HasValue)
                        throw new InvalidOperationException("Tech company coordinates are missing.");

                    locationLat = techCompany.User.Latitude.Value;
                    locationLon = techCompany.User.Longitude.Value;
                }
                else if (clusterDto.DropoffLatitude.HasValue && clusterDto.DropoffLongitude.HasValue)
                {
                    locationLat = clusterDto.DropoffLatitude.Value;
                    locationLon = clusterDto.DropoffLongitude.Value;
                }
                else
                {
                    throw new InvalidOperationException("Unable to determine location for cluster.");
                }

                var response = await _deliveryPersonRepo.FindWithIncludesAsync(
                   d => d.IsAvailable,
                   d => d.User
               );

                var availableDrivers = response != null && response.Any()
                    ? response.Select(DeliveryPersonMapper.ToReadDTO).ToList()
                    : new List<DeliveryPersonReadDTO>();

                var candidates = availableDrivers
                    .Where(d => d.Latitude.HasValue && d.Longitude.HasValue)
                    .Select(d => new
                    {
                        Driver = d,
                        DistanceKm = _locationService.CalculateDistance(
                            locationLat,
                            locationLon,
                            d.Latitude.Value,
                            d.Longitude.Value
                        ),
                    })
                    .OrderBy(x => x.DistanceKm)
                    .ToList();

                if (!candidates.Any())
                {
                    _logger.LogInformation("AutoAssignDriverAsync: {CandidateCount} candidates with location found.", candidates.Count);
                }

                foreach (var candidate in candidates)
                {
                    var offer = new DeliveryOffer
                    {
                        Id = Guid.NewGuid().ToString(),
                        DeliveryId = delivery.Id,
                        ClusterId = clusterId,
                        DeliveryPersonId = candidate.Driver.Id,
                        Status = DeliveryOfferStatus.Pending,
                        CreatedAt = DateTime.Now,
                        ExpiryTime = DateTime.Now.Add(_settings.OfferExpiryTime),
                        IsActive = true,
                        OfferedPrice = 50
                    };

                    await _deliveryOfferRepo.AddAsync(offer);

                    if (!string.IsNullOrEmpty(candidate.Driver.UserId))
                    {
                        await _notificationService.SendNotificationAsync(
                            candidate.Driver.UserId,
                            NotificationType.DeliveryOfferCreated,
                            delivery.Id,
                            "Delivery",
                            delivery.TrackingNumber ?? delivery.Id,
                            $"{candidate.DistanceKm:F2} km from you"
                        );
                    }
                }

                await _deliveryOfferRepo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AutoAssignDriverAsync: Failed for Delivery {DeliveryId}, Cluster {ClusterId}", delivery?.Id, clusterId);
            }
        }

        public async Task<GeneralResponse<bool>> AssignDriverToClusterAsync(
            string clusterId,
            string driverId
        )
        {
            if (string.IsNullOrWhiteSpace(clusterId) || string.IsNullOrWhiteSpace(driverId))
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Cluster ID and Driver ID are required.",
                };

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var clusterResult = await _clusterService.GetByIdAsync(clusterId);
                if (!clusterResult.Success || clusterResult.Data == null)
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Cluster not found.",
                    };

                var cluster = clusterResult.Data;

                var delivery = await _deliveryRepo.GetByIdAsync(cluster.DeliveryId);
                if (delivery == null)
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Delivery not found.",
                    };

                // Get the offer for this driver
                var offer = await _deliveryOfferRepo.GetFirstOrDefaultAsync(o =>
                    o.ClusterId == clusterId
                    && o.DeliveryPersonId == driverId
                    && o.Status == DeliveryOfferStatus.Pending
                );

                if (offer == null)
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Offer not found or already accepted/expired.",
                    };

                // Accept this offer
                offer.Status = DeliveryOfferStatus.Accepted;
                offer.IsActive = false;
                _deliveryOfferRepo.Update(offer);

                // Expire all other offers for this cluster
                var otherOffers = await _deliveryOfferRepo.FindAsync(o =>
                    o.ClusterId == clusterId
                    && o.Id != offer.Id
                    && o.Status == DeliveryOfferStatus.Pending
                );
                foreach (var o in otherOffers)
                {
                    o.Status = DeliveryOfferStatus.Expired;
                    o.IsActive = false;
                    _deliveryOfferRepo.Update(o);
                }

                // Assign driver to cluster
                var assignResult = await _clusterService.AssignDriverAsync(clusterId, driverId);
                if (!assignResult.Success)
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = assignResult.Message,
                    };

                delivery.DeliveryPersonId = driverId;
                delivery.Status = DeliveryStatus.Assigned;
                _deliveryRepo.Update(delivery);

                await _deliveryOfferRepo.SaveChangesAsync();
                await _deliveryRepo.SaveChangesAsync();

                // Notify driver
                var notifyUserId = driverId;
                await _notificationService.SendNotificationAsync(
                    notifyUserId,
                    NotificationType.DeliveryOfferAccepted,
                    delivery.Id,
                    "Delivery",
                    delivery.TrackingNumber ?? delivery.Id
                );

                // Notify other drivers their offer expired
                foreach (var o in otherOffers)
                {
                    await _notificationService.SendNotificationAsync(
                        o.DeliveryPersonId,
                        NotificationType.DeliveryOfferExpired,
                        delivery.Id,
                        "Delivery",
                        delivery.TrackingNumber ?? delivery.Id
                    );
                }

                scope.Complete();

                _logger.LogInformation(
                    "AssignDriverToClusterAsync: Driver {DriverId} accepted cluster {ClusterId}.",
                    driverId,
                    clusterId
                );
                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = "Driver assigned successfully.",
                    Data = true,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "AssignDriverToClusterAsync: Failed to assign driver {DriverId} to cluster {ClusterId}.",
                    driverId,
                    clusterId
                );
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = $"Failed to assign driver: {ex.Message}",
                    Data = false,
                };
            }
        }

        public async Task<GeneralResponse<bool>> AcceptDeliveryAsync(
            string clusterId,
            string driverId
        )
        {
            if (string.IsNullOrWhiteSpace(clusterId) || string.IsNullOrWhiteSpace(driverId))
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Cluster ID and Driver ID are required.",
                };

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                var clusterResult = await _clusterService.GetByIdAsync(clusterId);
                if (!clusterResult.Success || clusterResult.Data == null)
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Cluster not found.",
                    };

                var cluster = clusterResult.Data;

                if (cluster.AssignedDriverId != driverId)
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Driver not assigned to this cluster.",
                    };

                var delivery = await _deliveryRepo.GetByIdWithIncludesAsync(cluster.DeliveryId, d => d.DeliveryPerson.User);
                if (delivery == null)
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Delivery not found.",
                    };

                var offer = (
                    await _deliveryOfferRepo.FindAsync(o =>
                        o.ClusterId == clusterId && o.DeliveryPersonId == driverId && o.IsActive
                    )
                ).FirstOrDefault();

                if (offer == null)
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "No active offer found.",
                    };

                offer.Status = DeliveryOfferStatus.Accepted;
                offer.RespondedAt = DateTime.Now;
                offer.IsActive = false;
                _deliveryOfferRepo.Update(offer);

                var driver =
                    (await _deliveryPersonService.GetByIdAsync(driverId)).Data
                    ?? throw new InvalidOperationException("Driver not found");

                bool isSplit = false;
                if (!string.IsNullOrWhiteSpace(cluster.TechCompanyId))
                {
                    var techCompany = await _techCompanyRepo.GetByIdAsync(cluster.TechCompanyId);
                    var distance = _locationService.CalculateDistance(
                        driver.CurrentLatitude.Value,
                        driver.CurrentLongitude.Value,
                        techCompany.User.Latitude.Value,
                        techCompany.User.Longitude.Value
                    );

                    if (distance > _settings.MaxDriverDistanceKm)
                    {
                        isSplit = true;
                        await _clusterService.SplitClusterAsync(delivery, cluster, driver);
                    }
                }

                delivery.Status = DeliveryStatus.Assigned;
                _deliveryRepo.Update(delivery);
                cluster.Status = DeliveryClusterStatus.Assigned;
                await _clusterService.UpdateClusterAsync(cluster.Id, cluster);

                await Task.WhenAll(
                    _deliveryRepo.SaveChangesAsync(),
                    _deliveryOfferRepo.SaveChangesAsync()
                );
                await _notificationService.SendNotificationAsync(
                    delivery.DeliveryPerson.User.Id,
                    NotificationType.DeliveryOfferAccepted,
                    delivery.Id,
                    "Delivery",
                    delivery.TrackingNumber ?? delivery.Id
                );

                scope.Complete();
                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = "Delivery offer accepted.",
                    Data = true,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "AcceptDeliveryAsync failed for driver {DriverId} on cluster {ClusterId}.",
                    driverId,
                    clusterId
                );
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = $"Failed to accept delivery: {ex.Message}",
                    Data = false,
                };
            }
        }

        public async Task<GeneralResponse<DeliveryClusterDTO>> UpdateClusterStatusAsync(
            string clusterId,
            DeliveryClusterStatus status,
            string? assignedDriverId = null
        )
        {
            var clusterResult = await _clusterService.GetByIdAsync(clusterId);
            if (!clusterResult.Success || clusterResult.Data == null)
                return new GeneralResponse<DeliveryClusterDTO>
                {
                    Success = false,
                    Message = "Cluster not found.",
                };

            var cluster = clusterResult.Data;
            var clusterDto = new DeliveryClusterDTO
            {
                Id = cluster.Id,
                DeliveryId = cluster.DeliveryId,
                TechCompanyId = cluster.TechCompanyId,
                TechCompanyName = cluster.TechCompanyName,
                DistanceKm = cluster.DistanceKm,
                Price = cluster.Price,
                Status = status,
                AssignedDriverId = assignedDriverId,
                AssignedDriverName = assignedDriverId != null ? cluster.AssignedDriverName : null,
                AssignmentTime = assignedDriverId != null ? DateTime.Now : null,
                DropoffLatitude = cluster.DropoffLatitude,
                DropoffLongitude = cluster.DropoffLongitude,
                SequenceOrder = cluster.SequenceOrder,
                DriverOfferCount = cluster.DriverOfferCount,
                Latitude = cluster.Latitude,
                Longitude = cluster.Longitude,
                EstimatedDistance = cluster.EstimatedDistance,
                EstimatedPrice = cluster.EstimatedPrice,
                PickupLatitude = cluster.PickupLatitude,
                PickupLongitude = cluster.PickupLongitude,
            };

            return await _clusterService.UpdateClusterAsync(clusterId, clusterDto);
        }

        public async Task<GeneralResponse<bool>> DeclineDeliveryAsync(
            string clusterId,
            string driverId
        )
        {
            if (string.IsNullOrWhiteSpace(clusterId) || string.IsNullOrWhiteSpace(driverId))
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Cluster ID and Driver ID are required.",
                };

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var clusterResult = await _clusterService.GetByIdAsync(clusterId);
                if (!clusterResult.Success || clusterResult.Data == null)
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Cluster not found.",
                    };

                var cluster = clusterResult.Data;
                if (cluster.AssignedDriverId != driverId)
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Driver not assigned to this cluster.",
                    };

                var delivery = await _deliveryRepo.GetByIdWithIncludesAsync(cluster.DeliveryId, c => c.DeliveryPerson.UserId);
                if (delivery == null)
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Delivery not found.",
                    };

                var offers = await _deliveryOfferRepo.FindAsync(o =>
                    o.ClusterId == clusterId && o.DeliveryPersonId == driverId && o.IsActive
                );
                var offer = offers.FirstOrDefault();
                if (offer == null)
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "No active offer found.",
                    };

                await UpdateOfferStatusAsync(offer, DeliveryOfferStatus.Declined);
                await UpdateClusterStatusAsync(clusterId, DeliveryClusterStatus.Pending, null);

                delivery.DeliveryPersonId = null;
                delivery.Status = DeliveryStatus.Pending;
                _deliveryRepo.Update(delivery);

                await _deliveryOfferRepo.SaveChangesAsync();
                await _deliveryRepo.SaveChangesAsync();

                if (delivery.RetryCount < _settings.MaxRetries)
                {
                    delivery.RetryCount++;
                    _deliveryRepo.Update(delivery);
                    await _deliveryRepo.SaveChangesAsync();

                    var lat =
                        delivery.DropoffLatitude
                        ?? throw new InvalidOperationException(
                            "Delivery dropoff latitude missing."
                        );
                    var lon =
                        delivery.DropoffLongitude
                        ?? throw new InvalidOperationException(
                            "Delivery dropoff longitude missing."
                        );
                    await AutoAssignDriverAsync(delivery, clusterId, lat, lon);
                }

                await _notificationService.SendNotificationAsync(
                    delivery.DeliveryPerson.UserId,
                    NotificationType.DeliveryOfferExpired,
                    delivery.Id,
                    "Delivery",
                    delivery.TrackingNumber ?? delivery.Id
                );

                scope.Complete();
                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = "Delivery offer declined.",
                    Data = true,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "DeclineDeliveryAsync failed for driver {DriverId} on cluster {ClusterId}.",
                    driverId,
                    clusterId
                );
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = $"Failed to decline delivery: {ex.Message}",
                    Data = false,
                };
            }
        }

        public async Task<GeneralResponse<bool>> CancelDeliveryAsync(string deliveryId)
        {
            if (string.IsNullOrWhiteSpace(deliveryId))
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Delivery ID is required.",
                };

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var delivery = await _deliveryRepo.GetByIdWithIncludesAsync(deliveryId, d => d.DeliveryPerson, d => d.Customer.UserId);
                if (delivery == null)
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Delivery not found.",
                    };

                if (
                    delivery.Status == DeliveryStatus.Delivered
                    || delivery.Status == DeliveryStatus.Cancelled
                )
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = $"Delivery is already {delivery.Status}.",
                    };

                delivery.Status = DeliveryStatus.Cancelled;
                delivery.UpdatedAt = DateTime.Now;
                _deliveryRepo.Update(delivery);

                var clustersResult = await _clusterService.GetByDeliveryIdAsync(deliveryId);
                if (clustersResult.Success && clustersResult.Data != null)
                {
                    foreach (var cluster in clustersResult.Data)
                        await UpdateClusterStatusAsync(cluster.Id, DeliveryClusterStatus.Cancelled);
                }

                var offers = (await _deliveryOfferRepo.GetAllAsync()).Where(o =>
                    o.DeliveryId == deliveryId && o.IsActive
                );
                foreach (var offer in offers)
                    await UpdateOfferStatusAsync(offer, DeliveryOfferStatus.Expired);

                await _deliveryOfferRepo.SaveChangesAsync();
                await _deliveryRepo.SaveChangesAsync();

                await _notificationService.SendNotificationAsync(
                    delivery.DeliveryPerson.UserId,
                    NotificationType.DeliveryCompleted,
                    delivery.Id,
                    "Delivery",
                    delivery.TrackingNumber ?? delivery.Id
                );

                await _notificationService.SendNotificationAsync(
                delivery.Customer.UserId,
                NotificationType.DeliveryCompleted,
                delivery.Id,
                "Delivery",
                delivery.TrackingNumber ?? delivery.Id
            );

                scope.Complete();
                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = "Delivery cancelled successfully.",
                    Data = true,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "CancelDeliveryAsync failed for delivery {DeliveryId}.",
                    deliveryId
                );
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = $"Failed to cancel delivery: {ex.Message}",
                    Data = false,
                };
            }
        }

        public async Task<GeneralResponse<bool>> CompleteDeliveryAsync(
            string deliveryId,
            string driverId
        )
        {
            if (string.IsNullOrWhiteSpace(deliveryId) || string.IsNullOrWhiteSpace(driverId))
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Delivery ID and Driver ID are required.",
                };

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var delivery = await _deliveryRepo.GetByIdWithIncludesAsync(deliveryId, d=>d.Customer.UserId, d => d.DeliveryPerson);
                if (delivery == null)
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Delivery not found.",
                    };

                if (delivery.DeliveryPersonId != driverId)
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Driver not assigned to this delivery.",
                    };

                if (
                    delivery.Status == DeliveryStatus.Delivered
                    || delivery.Status == DeliveryStatus.Cancelled
                )
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = $"Delivery is already {delivery.Status}.",
                    };

                delivery.Status = DeliveryStatus.Delivered;
                delivery.ActualDeliveryDate = DateTime.Now;
                delivery.UpdatedAt = DateTime.Now;
                _deliveryRepo.Update(delivery);

                var clustersResult = await _clusterService.GetByDeliveryIdAsync(deliveryId);
                if (clustersResult.Success && clustersResult.Data != null)
                {
                    foreach (var cluster in clustersResult.Data)
                        await UpdateClusterStatusAsync(
                            cluster.Id,
                            DeliveryClusterStatus.Completed,
                            cluster.AssignedDriverId
                        );
                }

                var offers = (await _deliveryOfferRepo.GetAllAsync()).Where(o =>
                    o.DeliveryId == deliveryId && o.IsActive
                );
                foreach (var offer in offers)
                    await UpdateOfferStatusAsync(offer, DeliveryOfferStatus.Accepted);

                await _deliveryOfferRepo.SaveChangesAsync();
                await _deliveryRepo.SaveChangesAsync();

                await _notificationService.SendNotificationAsync(
                    delivery.Customer.UserId,
                    NotificationType.DeliveryCompleted,
                    delivery.Id,
                    "Delivery",
                    delivery.TrackingNumber ?? delivery.Id
                );

                scope.Complete();
                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = "Delivery completed successfully.",
                    Data = true,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "CompleteDeliveryAsync failed for delivery {DeliveryId} by driver {DriverId}.",
                    deliveryId,
                    driverId
                );
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = $"Failed to complete delivery: {ex.Message}",
                    Data = false,
                };
            }
        }

        public async Task<GeneralResponse<DeliveryReadDTO>> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("GetByIdAsync: Invalid input - id is empty.");
                return new GeneralResponse<DeliveryReadDTO>
                {
                    Success = false,
                    Message = "Delivery ID is required.",
                };
            }

            try
            {
                var delivery = await _deliveryRepo.GetByIdAsync(id);
                if (delivery == null)
                {
                    _logger.LogWarning("GetByIdAsync: Delivery {DeliveryId} not found.", id);
                    return new GeneralResponse<DeliveryReadDTO>
                    {
                        Success = false,
                        Message = "Delivery not found.",
                    };
                }

                var clustersResult = await _clusterService.GetByDeliveryIdAsync(id);
                var clusters = clustersResult.Success
                    ? clustersResult.Data
                    : Enumerable.Empty<DeliveryClusterDTO>();

                var readDto = DeliveryMapper.ToReadDTO(delivery, clusters);
                _logger.LogInformation("GetByIdAsync: Retrieved delivery {DeliveryId}.", id);
                return new GeneralResponse<DeliveryReadDTO>
                {
                    Success = true,
                    Message = "Delivery retrieved successfully.",
                    Data = readDto,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync: Failed to retrieve delivery {DeliveryId}.", id);
                return new GeneralResponse<DeliveryReadDTO>
                {
                    Success = false,
                    Message = $"Failed to retrieve delivery: {ex.Message}",
                };
            }
        }

        public async Task<GeneralResponse<IEnumerable<DeliveryReadDTO>>> GetAllAsync()
        {
            try
            {
                var deliveries = await _deliveryRepo.GetAllAsync();
                var assignedDeliveries = deliveries.Where(d => d.Status == DeliveryStatus.Assigned).ToList();

                var result = new List<DeliveryReadDTO>();

                foreach (var delivery in assignedDeliveries)
                {
                    var clustersResult = await _clusterService.GetByDeliveryIdAsync(delivery.Id);
                    var clusters = clustersResult.Success
                        ? clustersResult.Data
                        : Enumerable.Empty<DeliveryClusterDTO>();
                    result.Add(DeliveryMapper.ToReadDTO(delivery, clusters));
                }

                _logger.LogInformation(
                    "GetAllAsync: Retrieved {DeliveryCount} deliveries.",
                    result.Count
                );
                return new GeneralResponse<IEnumerable<DeliveryReadDTO>>
                {
                    Success = true,
                    Message = "Deliveries retrieved successfully.",
                    Data = result,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllAsync: Failed to retrieve deliveries.");
                return new GeneralResponse<IEnumerable<DeliveryReadDTO>>
                {
                    Success = false,
                    Message = $"Failed to retrieve deliveries: {ex.Message}",
                    Data = Enumerable.Empty<DeliveryReadDTO>(),
                };
            }
        }

        public async Task<GeneralResponse<bool>> DeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("DeleteAsync: Invalid input - id is empty.");
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Delivery ID is required.",
                };
            }

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var delivery = await _deliveryRepo.GetByIdAsync(id);
                if (delivery == null)
                {
                    _logger.LogWarning("DeleteAsync: Delivery {DeliveryId} not found.", id);
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Delivery not found.",
                    };
                }

                if (delivery.Status == DeliveryStatus.Delivered)
                {
                    _logger.LogWarning(
                        "DeleteAsync: Cannot delete delivered delivery {DeliveryId}.",
                        id
                    );
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Cannot delete a delivered delivery.",
                    };
                }

                delivery.Status = DeliveryStatus.Cancelled;
                delivery.UpdatedAt = DateTime.Now;
                _deliveryRepo.Update(delivery);

                var clustersResult = await _clusterService.GetByDeliveryIdAsync(id);
                if (clustersResult.Success && clustersResult.Data != null)
                {
                    foreach (var cluster in clustersResult.Data)
                    {
                        var deleteResult = await _clusterService.DeleteClusterAsync(cluster.Id);
                        if (!deleteResult.Success)
                        {
                            _logger.LogError(
                                "DeleteAsync: Failed to delete cluster {ClusterId}: {Message}",
                                cluster.Id,
                                deleteResult.Message
                            );
                            return new GeneralResponse<bool>
                            {
                                Success = false,
                                Message = $"Failed to delete cluster: {deleteResult.Message}",
                            };
                        }
                    }
                }

                var offers = (await _deliveryOfferRepo.GetAllAsync()).Where(o =>
                    o.DeliveryId == id && o.IsActive
                );
                foreach (var offer in offers)
                {
                    offer.Status = DeliveryOfferStatus.Expired;
                    offer.IsActive = false;
                    offer.RespondedAt = DateTime.Now;
                    _deliveryOfferRepo.Update(offer);
                }

                await _deliveryOfferRepo.SaveChangesAsync();
                await _deliveryRepo.SaveChangesAsync();

                scope.Complete();
                _logger.LogInformation(
                    "DeleteAsync: Delivery {DeliveryId} deleted (cancelled) successfully.",
                    id
                );
                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = "Delivery deleted successfully.",
                    Data = true,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync: Failed to delete delivery {DeliveryId}.", id);
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = $"Failed to delete delivery: {ex.Message}",
                    Data = false,
                };
            }
        }

        public async Task<
            GeneralResponse<IEnumerable<DeliveryPersonReadDTO>>
        > GetAvailableDeliveryPersonsAsync()
        {
            try
            {
                var drivers = await _deliveryPersonRepo.FindWithIncludesAsync(
                    dp => dp.IsAvailable,
                    dp => dp.User,
                    dp => dp.Role
                );

                if (drivers == null || !drivers.Any())
                    return new GeneralResponse<IEnumerable<DeliveryPersonReadDTO>>
                    {
                        Success = true,
                        Data = new List<DeliveryPersonReadDTO>(),
                    };

                var dtoList = drivers
                    .Select(d => new DeliveryPersonReadDTO
                    {
                        Id = d.Id,
                        UserId = d.UserId,
                        RoleId = d.RoleId,
                        VehicleNumber = d.VehicleNumber,
                        VehicleType = d.VehicleType,
                        VehicleImage = d.VehicleImage,
                        AccountStatus = d.AccountStatus,
                        PhoneNumber = d.User?.PhoneNumber,
                        City = d.User?.City,
                        Country = d.User?.Country,
                        IsAvailable = d.IsAvailable,
                        UserName = d.User?.UserName,
                        UserFullName = d.User?.FullName,
                        RoleName = d.Role?.Name,
                        CurrentLatitude = d.CurrentLatitude,
                        CurrentLongitude = d.CurrentLongitude,
                    })
                    .ToList();

                return new GeneralResponse<IEnumerable<DeliveryPersonReadDTO>>
                {
                    Success = true,
                    Data = dtoList,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get available delivery persons.");
                return new GeneralResponse<IEnumerable<DeliveryPersonReadDTO>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null,
                };
            }
        }

        public async Task<GeneralResponse<DeliveryTrackingDTO>> GetDeliveryTrackingAsync(
            string deliveryId
        )
        {
            if (string.IsNullOrWhiteSpace(deliveryId))
            {
                _logger.LogWarning(
                    "GetDeliveryTrackingAsync: Invalid input - deliveryId is empty."
                );
                return new GeneralResponse<DeliveryTrackingDTO>
                {
                    Success = false,
                    Message = "Delivery ID is required.",
                };
            }

            try
            {
                var delivery = await _deliveryRepo.GetByIdAsync(deliveryId);
                if (delivery == null)
                {
                    _logger.LogWarning(
                        "GetDeliveryTrackingAsync: Delivery {DeliveryId} not found.",
                        deliveryId
                    );
                    return new GeneralResponse<DeliveryTrackingDTO>
                    {
                        Success = false,
                        Message = "Delivery not found.",
                    };
                }

                var clustersResult = await _clusterService.GetByDeliveryIdAsync(deliveryId);
                var clusterTracking = new List<DeliveryClusterTrackingDTO>();
                if (clustersResult.Success && clustersResult.Data != null)
                {
                    foreach (var cluster in clustersResult.Data)
                    {
                        var trackingResult = await _clusterService.GetTrackingAsync(cluster.Id);
                        if (trackingResult.Success && trackingResult.Data?.Tracking != null)
                        {
                            clusterTracking.Add(trackingResult.Data.Tracking);
                        }
                    }
                }

                var trackingDto = new DeliveryTrackingDTO
                {
                    DeliveryId = delivery.Id,
                    Status = delivery.Status,
                    CurrentLat = delivery.DropoffLatitude,
                    CurrentLng = delivery.DropoffLongitude,
                    EstimatedArrival = delivery.EstimatedDeliveryDate,
                    Clusters = clusterTracking,
                };

                _logger.LogInformation(
                    "GetDeliveryTrackingAsync: Retrieved tracking for delivery {DeliveryId}.",
                    deliveryId
                );
                return new GeneralResponse<DeliveryTrackingDTO>
                {
                    Success = true,
                    Message = "Tracking retrieved successfully.",
                    Data = trackingDto,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "GetDeliveryTrackingAsync: Failed to retrieve tracking for delivery {DeliveryId}.",
                    deliveryId
                );
                return new GeneralResponse<DeliveryTrackingDTO>
                {
                    Success = false,
                    Message = $"Failed to retrieve tracking: {ex.Message}",
                    Data = null,
                };
            }
        }

        public async Task<IEnumerable<Delivery>> GetDeliveriesWithExpiredOffersAsync()
        {
            return await _deliveryRepo.FindWithIncludesAsync(
                d =>
                    d.Status == DeliveryStatus.Pending
                    && d.Offers.Any(o => o.IsActive && o.ExpiryTime <= DateTime.Now),
                d => d.Offers
            );
        }

        private async Task UpdateOfferStatusAsync(DeliveryOffer offer, DeliveryOfferStatus status)
        {
            if (offer == null)
                return;
            offer.Status = status;
            offer.RespondedAt = DateTime.Now;
            offer.IsActive = false;
            _deliveryOfferRepo.Update(offer);
        }
    }
}
