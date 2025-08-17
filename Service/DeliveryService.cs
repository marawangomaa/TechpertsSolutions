using Core.DTOs;
using Core.DTOs.DeliveryDTOs;
using Core.DTOs.DeliveryPersonDTOs;
using Core.Entities;
using Core.Enums;
using Core.Interfaces;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;
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
            ILogger<DeliveryService> logger)
        {
            _orderRepo = orderRepo ?? throw new ArgumentNullException(nameof(orderRepo));
            _deliveryRepo = deliveryRepo ?? throw new ArgumentNullException(nameof(deliveryRepo));
            _deliveryOfferRepo = deliveryOfferRepo ?? throw new ArgumentNullException(nameof(deliveryOfferRepo));
            _deliveryPersonRepo = deliveryPersonRepo ?? throw new ArgumentNullException(nameof(deliveryPersonRepo));
            _techCompanyRepo = techCompanyRepo ?? throw new ArgumentNullException(nameof(techCompanyRepo));
            _deliveryClusterRepo = deliveryClusterRepo ?? throw new ArgumentNullException(nameof(deliveryClusterRepo));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _locationService = locationService ?? throw new ArgumentNullException(nameof(locationService));
            _deliveryPersonService = deliveryPersonService ?? throw new ArgumentNullException(nameof(deliveryPersonService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private IDeliveryClusterService _clusterService => _serviceProvider.GetRequiredService<IDeliveryClusterService>();

        //public async Task<GeneralResponse<DeliveryReadDTO>> CreateAsync(DeliveryCreateDTO dto)
        //{
        //    if (dto == null || string.IsNullOrWhiteSpace(dto.OrderId) || dto.CustomerLatitude < 0 || dto.CustomerLongitude < 0)
        //    {
        //        _logger.LogWarning("CreateAsync: Invalid input - DTO is null, OrderId is empty, or customer location missing.");
        //        return new GeneralResponse<DeliveryReadDTO>
        //        {
        //            Success = false,
        //            Message = "Delivery data, OrderId, and customer location are required.",
        //            Data = null
        //        };
        //    }

        //    using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        //    try
        //    {
        //        // Map and create delivery, set dropoff to customer location
        //        var delivery = DeliveryMapper.ToEntity(dto);
        //        delivery.DropoffLatitude = dto.CustomerLatitude;
        //        delivery.DropoffLongitude = dto.CustomerLongitude;
        //        await _deliveryRepo.AddAsync(delivery);
        //        await _deliveryRepo.SaveChangesAsync();

        //        var createdClusters = new List<DeliveryClusterDTO>();

        //        // Handle single or multi-company orders via clusters
        //        foreach (var clusterDto in dto.Clusters ?? new List<DeliveryClusterCreateDTO>())
        //        {
        //            // Set dropoff to customer for all clusters
        //            clusterDto.DropoffLatitude = dto.CustomerLatitude;
        //            clusterDto.DropoffLongitude = dto.CustomerLongitude;

        //            var clusterResult = await _clusterService.CreateClusterAsync(delivery.Id, clusterDto);

        //            if (!clusterResult.Success)
        //            {
        //                _logger.LogError("CreateAsync: Cluster creation failed for delivery {DeliveryId}: {Message}", delivery.Id, clusterResult.Message);
        //                return new GeneralResponse<DeliveryReadDTO>
        //                {
        //                    Success = false,
        //                    Message = $"Cluster creation failed: {clusterResult.Message}",
        //                    Data = null
        //                };
        //            }

        //            var createdCluster = clusterResult.Data;
        //            createdClusters.Add(createdCluster);

        //            if (string.IsNullOrWhiteSpace(createdCluster.AssignedDriverId))
        //            {
        //                // Auto-assign nearest driver from customer location
        //                await AutoAssignDriverAsync(delivery, createdCluster.Id, dto.CustomerLatitude, dto.CustomerLongitude);
        //            }
        //        }

        //        var readDto = DeliveryMapper.ToReadDTO(delivery, createdClusters);
        //        scope.Complete();
        //        _logger.LogInformation("CreateAsync: Delivery {DeliveryId} created successfully with {ClusterCount} clusters.", delivery.Id, createdClusters.Count);
        //        return new GeneralResponse<DeliveryReadDTO>
        //        {
        //            Success = true,
        //            Message = "Delivery created successfully.",
        //            Data = readDto
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "CreateAsync: Failed to create delivery for OrderId {OrderId}.", dto.OrderId);
        //        return new GeneralResponse<DeliveryReadDTO>
        //        {
        //            Success = false,
        //            Message = $"Failed to create delivery: {ex.Message}",
        //            Data = null
        //        };
        //    }
        //}


        //public async Task<GeneralResponse<DeliveryReadDTO>> CreateAsync(DeliveryCreateDTO dto)
        //{
        //    if (dto == null || string.IsNullOrWhiteSpace(dto.OrderId) || dto.CustomerLatitude < 0 || dto.CustomerLongitude < 0)
        //    {
        //        _logger.LogWarning("CreateAsync: Invalid input - DTO is null, OrderId is empty, or customer location missing.");
        //        return new GeneralResponse<DeliveryReadDTO>
        //        {
        //            Success = false,
        //            Message = "Delivery data, OrderId, and customer location are required.",
        //            Data = null
        //        };
        //    }

        //    using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        //    try
        //    {
        //        var order = await _orderRepo.GetOrderEntityByIdAsync(dto.OrderId);

        //        if (order == null)
        //        {
        //            _logger.LogWarning("CreateAsync: Order {OrderId} not found.", dto.OrderId);
        //            return new GeneralResponse<DeliveryReadDTO>
        //            {
        //                Success = false,
        //                Message = "Order not found.",
        //                Data = null
        //            };
        //        }

        //        var distinctCompanies = order.OrderItems
        //            .Select(i => i.Product?.TechCompanyId)
        //            .Where(id => !string.IsNullOrWhiteSpace(id))
        //            .Distinct()
        //            .ToList();

        //        bool isComplex = distinctCompanies.Count > 1;
        //        _logger.LogInformation("CreateAsync: Order {OrderId} is {OrderType} ({CompanyCount} companies).",
        //            order.Id, isComplex ? "Complex" : "Easy", distinctCompanies.Count);

        //        var delivery = DeliveryMapper.ToEntity(dto);
        //        delivery.OrderId = order.Id;
        //        delivery.CustomerId = order.CustomerId;
        //        delivery.DropoffLatitude = dto.CustomerLatitude;
        //        delivery.DropoffLongitude = dto.CustomerLongitude;

        //        await _deliveryRepo.AddAsync(delivery);
        //        await _deliveryRepo.SaveChangesAsync();

        //        var createdClusters = new List<DeliveryClusterDTO>();

        //        async Task AssignDriverWithDistanceCheckAsync(DeliveryClusterDTO cluster, double pickupLat, double pickupLng)
        //        {
        //            if (!_settings.AssignNearestDriverFirst) return;

        //            var availableDriversResponse = await _deliveryPersonService.GetAvailableDeliveryPersonsAsync();

        //            var nearestDriver = availableDriversResponse.Data?
        //                .OrderBy(d => _locationService.CalculateDistance((double)d.CurrentLatitude, (double)d.CurrentLongitude, pickupLat, pickupLng))
        //                .FirstOrDefault();

        //            if (nearestDriver != null)
        //            {
        //                double distanceToPickup = _locationService.CalculateDistance(
        //                    (double)nearestDriver.CurrentLatitude, (double)nearestDriver.CurrentLongitude, pickupLat, pickupLng
        //                );

        //                if (_settings.EnableReassignment && distanceToPickup > _settings.MaxDriverDistanceKm)
        //                {
        //                    _logger.LogWarning("Driver {DriverId} too far ({Distance} km) from pickup {ClusterId}, reassigning...",
        //                        nearestDriver.Id, distanceToPickup, cluster.Id);

        //                    await CancelDeliveryAsync(nearestDriver.Id);

        //                    await AutoAssignDriverAsync(delivery, cluster.Id, pickupLat, pickupLng);
        //                }
        //            }
        //        }

        //        var companies = isComplex
        //            ? order.OrderItems.Select(i => i.Product.TechCompany).Distinct().ToList()
        //            : new List<TechCompany> { order.OrderItems.First().Product.TechCompany };

        //        foreach (var company in companies)
        //        {
        //            var clusterDto = new DeliveryClusterCreateDTO
        //            {
        //                TechCompanyId = company.Id,
        //                TechCompanyName = company.Name,
        //                DropoffLatitude = dto.CustomerLatitude,
        //                DropoffLongitude = dto.CustomerLongitude
        //            };

        //            var clusterResult = await _clusterService.CreateClusterAsync(delivery.Id, clusterDto);
        //            if (!clusterResult.Success)
        //            {
        //                _logger.LogError("CreateAsync: Cluster creation failed for delivery {DeliveryId}: {Message}", delivery.Id, clusterResult.Message);
        //                return new GeneralResponse<DeliveryReadDTO>
        //                {
        //                    Success = false,
        //                    Message = $"Cluster creation failed: {clusterResult.Message}",
        //                    Data = null
        //                };
        //            }

        //            var createdCluster = clusterResult.Data;
        //            createdClusters.Add(createdCluster);

        //            await AssignDriverWithDistanceCheckAsync(createdCluster, company.Latitude, company.Longitude);
        //        }

        //        var readDto = DeliveryMapper.ToReadDTO(delivery, createdClusters);
        //        scope.Complete();

        //        _logger.LogInformation("CreateAsync: Delivery {DeliveryId} created successfully with {ClusterCount} clusters.", delivery.Id, createdClusters.Count);

        //        return new GeneralResponse<DeliveryReadDTO>
        //        {
        //            Success = true,
        //            Message = "Delivery created successfully.",
        //            Data = readDto
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "CreateAsync: Failed to create delivery for OrderId {OrderId}.", dto.OrderId);
        //        return new GeneralResponse<DeliveryReadDTO>
        //        {
        //            Success = false,
        //            Message = $"Failed to create delivery: {ex.Message}",
        //            Data = null
        //        };
        //    }
        //}

        //public async Task<GeneralResponse<DeliveryReadDTO>> CreateAsync(DeliveryCreateDTO dto)
        //{
        //    if (dto == null || string.IsNullOrWhiteSpace(dto.OrderId) ||
        //        dto.CustomerLatitude < -90 || dto.CustomerLatitude > 90 ||
        //        dto.CustomerLongitude < -180 || dto.CustomerLongitude > 180)
        //    {
        //        _logger.LogWarning("CreateAsync: Invalid input - DTO is null, OrderId is empty, or customer location missing.");
        //        return new GeneralResponse<DeliveryReadDTO>
        //        {
        //            Success = false,
        //            Message = "Delivery data, OrderId, and customer location are required.",
        //            Data = null
        //        };
        //    }

        //    using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        //    try
        //    {
        //        var order = await _orderRepo.GetFirstOrDefaultAsync(
        //            o => o.Id == dto.OrderId,
        //            query => query
        //                .Include(o => o.OrderItems)
        //                    .ThenInclude(oi => oi.Product)
        //                        .ThenInclude(p => p.TechCompany)
        //        );

        //        if (order == null)
        //        {
        //            _logger.LogWarning("CreateAsync: Order {OrderId} not found.", dto.OrderId);
        //            return new GeneralResponse<DeliveryReadDTO>
        //            {
        //                Success = false,
        //                Message = "Order not found.",
        //                Data = null
        //            };
        //        }

        //        var distinctCompanies = order.OrderItems
        //            .Select(i => i.Product?.TechCompany)
        //            .Where(c => c != null)
        //            .DistinctBy(c => c!.Id) // safer distinct
        //            .ToList();

        //        bool isComplex = distinctCompanies.Count > 1;
        //        _logger.LogInformation("CreateAsync: Order {OrderId} is {OrderType} ({CompanyCount} companies).",
        //            order.Id, isComplex ? "Complex" : "Easy", distinctCompanies.Count);

        //        var delivery = DeliveryMapper.ToEntity(dto);
        //        delivery.OrderId = order.Id;
        //        delivery.CustomerId = order.CustomerId;
        //        delivery.DropoffLatitude = dto.CustomerLatitude;
        //        delivery.DropoffLongitude = dto.CustomerLongitude;

        //        await _deliveryRepo.AddAsync(delivery);
        //        await _deliveryRepo.SaveChangesAsync();

        //        async Task AssignDriverWithDistanceCheckAsync(DeliveryClusterDTO cluster, double pickupLat, double pickupLng)
        //        {
        //            if (!_settings.AssignNearestDriverFirst) return;

        //            var availableDriversResp = await _deliveryPersonService.GetAvailableDeliveryPersonsAsync();
        //            var nearestDriver = availableDriversResp.Data?
        //                .Where(d => d.CurrentLatitude.HasValue && d.CurrentLongitude.HasValue)
        //                .OrderBy(d => _locationService.CalculateDistance(
        //                    d.CurrentLatitude.Value,
        //                    d.CurrentLongitude.Value,
        //                    pickupLat,
        //                    pickupLng))
        //                .FirstOrDefault();

        //            if (nearestDriver != null)
        //            {
        //                double distanceToPickup = _locationService.CalculateDistance(
        //                    nearestDriver.CurrentLatitude.Value,
        //                    nearestDriver.CurrentLongitude.Value,
        //                    pickupLat,
        //                    pickupLng
        //                );

        //                if (_settings.EnableReassignment && distanceToPickup > _settings.MaxDriverDistanceKm)
        //                {
        //                    _logger.LogWarning("Driver {DriverId} too far ({Distance} km) from cluster {ClusterId}, reassigning...",
        //                        nearestDriver.Id, distanceToPickup, cluster.Id);

        //                    await CancelDeliveryAsync(cluster.Id);
        //                    await AutoAssignDriverAsync(delivery, cluster.Id, pickupLat, pickupLng);
        //                }
        //                else
        //                {
        //                    await AutoAssignDriverAsync(delivery, cluster.Id, pickupLat, pickupLng);
        //                }
        //            }
        //            else
        //            {
        //                _logger.LogWarning("No available drivers found for cluster {ClusterId}", cluster.Id);
        //            }
        //        }

        //        var companies = isComplex ? distinctCompanies : new List<TechCompany> { distinctCompanies.First() };

        //        // ===== Step 1: Create clusters =====
        //        var clusterTasks = companies.Select(async company =>
        //        {
        //            var clusterDto = new DeliveryClusterCreateDTO
        //            {
        //                TechCompanyId = company.Id,
        //                TechCompanyName = company.User.FullName,
        //                DropoffLatitude = dto.CustomerLatitude,
        //                DropoffLongitude = dto.CustomerLongitude
        //            };

        //            var clusterResult = await _clusterService.CreateClusterAsync(delivery.Id, clusterDto);
        //            if (!clusterResult.Success)
        //            {
        //                _logger.LogError("CreateAsync: Cluster creation failed for delivery {DeliveryId}: {Message}", delivery.Id, clusterResult.Message);
        //                throw new InvalidOperationException($"Cluster creation failed: {clusterResult.Message}");
        //            }

        //            return clusterResult.Data;
        //        });

        //        var clusters = (await Task.WhenAll(clusterTasks)).ToList();

        //        // ===== Step 2: Assign drivers =====
        //        var assignTasks = clusters.Select(async cluster =>
        //        {
        //            var company = companies.First(c => c.Id == cluster.TechCompanyId);
        //            double pickupLat = company.User.Latitude ?? dto.CustomerLatitude;
        //            double pickupLng = company.User.Longitude ?? dto.CustomerLongitude;

        //            await AssignDriverWithDistanceCheckAsync(cluster, pickupLat, pickupLng);
        //        });

        //        await Task.WhenAll(assignTasks);

        //        var readDto = DeliveryMapper.ToReadDTO(delivery, clusters);
        //        scope.Complete();

        //        _logger.LogInformation("CreateAsync: Delivery {DeliveryId} created successfully with {ClusterCount} clusters.",
        //            delivery.Id, clusters.Count);

        //        return new GeneralResponse<DeliveryReadDTO>
        //        {
        //            Success = true,
        //            Message = "Delivery created successfully.",
        //            Data = readDto
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "CreateAsync: Failed to create delivery for OrderId {OrderId}.", dto.OrderId);
        //        return new GeneralResponse<DeliveryReadDTO>
        //        {
        //            Success = false,
        //            Message = $"Failed to create delivery: {ex.Message}",
        //            Data = null
        //        };
        //    }
        //}

        public async Task<GeneralResponse<DeliveryReadDTO>> CreateAsync(DeliveryCreateDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.OrderId) ||
                dto.CustomerLatitude < -90 || dto.CustomerLatitude > 90 ||
                dto.CustomerLongitude < -180 || dto.CustomerLongitude > 180)
            {
                _logger.LogWarning("CreateAsync: Invalid input - DTO is null, OrderId is empty, or customer location missing.");
                return new GeneralResponse<DeliveryReadDTO>
                {
                    Success = false,
                    Message = "Delivery data, OrderId, and customer location are required.",
                    Data = null
                };
            }

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                var order = await _orderRepo.GetFirstOrDefaultAsync(
                    o => o.Id == dto.OrderId,
                    query => query.Include(o => o.OrderItems)
                                  .ThenInclude(oi => oi.Product)
                                  .ThenInclude(p => p.TechCompany)
                );

                if (order == null)
                {
                    _logger.LogWarning("CreateAsync: Order {OrderId} not found.", dto.OrderId);
                    return new GeneralResponse<DeliveryReadDTO>
                    {
                        Success = false,
                        Message = "Order not found.",
                        Data = null
                    };
                }

                var distinctCompanies = order.OrderItems
                    .Select(i => i.Product?.TechCompany)
                    .Where(c => c != null)
                    .Distinct()
                    .ToList();

                bool isComplex = distinctCompanies.Count > 1;
                _logger.LogInformation("CreateAsync: Order {OrderId} is {OrderType} ({CompanyCount} companies).",
                    order.Id, isComplex ? "Complex" : "Easy", distinctCompanies.Count);

                var delivery = DeliveryMapper.ToEntity(dto);
                delivery.OrderId = order.Id;
                delivery.CustomerId = order.CustomerId;
                delivery.DropoffLatitude = dto.CustomerLatitude;
                delivery.DropoffLongitude = dto.CustomerLongitude;

                await _deliveryRepo.AddAsync(delivery);
                await _deliveryRepo.SaveChangesAsync();

                var createdClusters = new List<DeliveryClusterDTO>();

                async Task AssignDriverWithDistanceCheckAsync(DeliveryClusterDTO cluster, double pickupLat, double pickupLng)
                {
                    if (!_settings.AssignNearestDriverFirst) return;

                    var availableDrivers = await _deliveryPersonService.GetAvailableDeliveryPersonsAsync();
                    var nearestDriver = availableDrivers.Data?
                        .Where(d => d.Latitude.HasValue && d.Longitude.HasValue)
                        .OrderBy(d => _locationService.CalculateDistance(
                            d.Latitude.Value,
                            d.Longitude.Value,
                            pickupLat,
                            pickupLng))
                        .FirstOrDefault();

                    if (nearestDriver != null)
                    {
                        double distanceToPickup = _locationService.CalculateDistance(
                            nearestDriver.Latitude.Value,
                            nearestDriver.Longitude.Value,
                            pickupLat,
                            pickupLng
                        );

                        if (_settings.EnableReassignment && distanceToPickup > _settings.MaxDriverDistanceKm)
                        {
                            _logger.LogWarning("Driver {DriverId} too far ({Distance} km) from cluster {ClusterId}, reassigning...",
                                nearestDriver.Id, distanceToPickup, cluster.Id);

                            await CancelDeliveryAsync(cluster.Id);
                            await AutoAssignDriverAsync(delivery, cluster.Id, pickupLat, pickupLng);
                        }
                        else
                        {
                            await AutoAssignDriverAsync(delivery, cluster.Id, pickupLat, pickupLng);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No available drivers found for cluster {ClusterId}", cluster.Id);
                    }
                }

                if (!isComplex)
                {
                    var company = distinctCompanies.First();
                    var availableDriversResp = await _deliveryPersonService.GetAvailableDeliveryPersonsAsync();
                    var nearestDriver = availableDriversResp.Data?
                        .Where(d => d.CurrentLatitude.HasValue && d.CurrentLongitude.HasValue)
                        .OrderBy(d => _locationService.CalculateDistance(
                            d.CurrentLatitude.Value,
                            d.CurrentLongitude.Value,
                            company.User.Latitude ?? dto.CustomerLatitude,
                            company.User.Longitude ?? dto.CustomerLongitude))
                        .FirstOrDefault();

                    double distance = nearestDriver != null
                        ? _locationService.CalculateDistance(
                            nearestDriver.Latitude.Value,
                            nearestDriver.Longitude.Value,
                            company.User.Latitude ?? dto.CustomerLatitude,
                            company.User.Longitude ?? dto.CustomerLongitude)
                        : double.MaxValue;

                    if (distance <= _settings.MaxDriverDistanceKm)
                    {
                        _logger.LogInformation("Single company and driver within max distance, skipping cluster creation.");
                        await AutoAssignDriverAsync(delivery, "default-cluster-id", company.User.Latitude, company.User.Longitude);
                        var readDto = DeliveryMapper.ToReadDTO(delivery, createdClusters);
                        scope.Complete();
                        return new GeneralResponse<DeliveryReadDTO>
                        {
                            Success = true,
                            Message = "Delivery created and driver auto-assigned.",
                            Data = readDto
                        };
                    }
                }

                var clusterTasks = distinctCompanies.Select(async company =>
                {
                    var clusterDto = new DeliveryClusterCreateDTO
                    {
                        TechCompanyId = company.Id,
                        TechCompanyName = company.User.FullName,
                        DropoffLatitude = dto.CustomerLatitude,
                        DropoffLongitude = dto.CustomerLongitude
                    };

                    var clusterResult = await _clusterService.CreateClusterAsync(delivery.Id, clusterDto);
                    if (!clusterResult.Success)
                        throw new InvalidOperationException($"Cluster creation failed: {clusterResult.Message}");

                    return clusterResult.Data;
                });

                var clusters = (await Task.WhenAll(clusterTasks)).ToList();

                var assignTasks = clusters.Select(cluster =>
                {
                    var company = distinctCompanies.First(c => c.Id == cluster.TechCompanyId);
                    double pickupLat = company.User.Latitude ?? dto.CustomerLatitude;
                    double pickupLng = company.User.Longitude ?? dto.CustomerLongitude;
                    return AssignDriverWithDistanceCheckAsync(cluster, pickupLat, pickupLng);
                });

                await Task.WhenAll(assignTasks);
                createdClusters = clusters;

                var finalDto = DeliveryMapper.ToReadDTO(delivery, createdClusters);
                scope.Complete();

                _logger.LogInformation("CreateAsync: Delivery {DeliveryId} created successfully with {ClusterCount} clusters.",
                    delivery.Id, createdClusters.Count);

                return new GeneralResponse<DeliveryReadDTO>
                {
                    Success = true,
                    Message = "Delivery created successfully.",
                    Data = finalDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateAsync: Failed to create delivery for OrderId {OrderId}.", dto.OrderId);
                return new GeneralResponse<DeliveryReadDTO>
                {
                    Success = false,
                    Message = $"Failed to create delivery: {ex.Message}",
                    Data = null
                };
            }
        }

        //public async Task<GeneralResponse<bool>> AssignDriverToClusterAsync(string clusterId, string driverId)
        //{
        //    if (string.IsNullOrWhiteSpace(clusterId) || string.IsNullOrWhiteSpace(driverId))
        //    {
        //        _logger.LogWarning("AssignDriverToClusterAsync: Invalid input - clusterId or driverId is empty.");
        //        return new GeneralResponse<bool> { Success = false, Message = "Cluster ID and Driver ID are required." };
        //    }

        //    using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        //    try
        //    {
        //        var clusterResult = await _clusterService.GetByIdAsync(clusterId);
        //        if (!clusterResult.Success || clusterResult.Data == null)
        //        {
        //            _logger.LogWarning("AssignDriverToClusterAsync: Cluster {ClusterId} not found.", clusterId);
        //            return new GeneralResponse<bool> { Success = false, Message = "Cluster not found." };
        //        }

        //        var cluster = clusterResult.Data;
        //        var delivery = await _deliveryRepo.GetByIdAsync(cluster.DeliveryId);
        //        if (delivery == null)
        //        {
        //            _logger.LogWarning("AssignDriverToClusterAsync: Delivery {DeliveryId} not found for cluster {ClusterId}.", cluster.DeliveryId, clusterId);
        //            return new GeneralResponse<bool> { Success = false, Message = "Delivery not found." };
        //        }

        //        var assignResult = await _clusterService.AssignDriverAsync(clusterId, driverId);
        //        if (!assignResult.Success)
        //        {
        //            _logger.LogError("AssignDriverToClusterAsync: Failed to assign driver {DriverId} to cluster {ClusterId}: {Message}", driverId, clusterId, assignResult.Message);
        //            return new GeneralResponse<bool> { Success = false, Message = assignResult.Message };
        //        }

        //        delivery.DeliveryPersonId = driverId;
        //        delivery.Status = DeliveryStatus.Assigned;
        //        _deliveryRepo.Update(delivery);

        //        var offer = new DeliveryOffer
        //        {
        //            Id = Guid.NewGuid().ToString(),
        //            DeliveryId = delivery.Id,
        //            ClusterId = clusterId,  // Link to cluster
        //            DeliveryPersonId = driverId,
        //            Status = DeliveryOfferStatus.Pending,
        //            CreatedAt = DateTime.UtcNow,
        //            ExpiryTime = DateTime.UtcNow.Add(_settings.OfferExpiryTime),
        //            IsActive = true
        //        };
        //        await _deliveryOfferRepo.AddAsync(offer);
        //        await _deliveryOfferRepo.SaveChangesAsync();

        //        await _notificationService.SendNotificationAsync(
        //            driverId,
        //            $"New delivery offer: #{delivery.TrackingNumber ?? delivery.Id} assigned to you.",
        //            NotificationType.DeliveryOfferCreated,
        //            delivery.Id,
        //            "Delivery");

        //        scope.Complete();
        //        _logger.LogInformation("AssignDriverToClusterAsync: Driver {DriverId} assigned to cluster {ClusterId}.", driverId, clusterId);
        //        return new GeneralResponse<bool> { Success = true, Message = "Driver assigned successfully.", Data = true };
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "AssignDriverToClusterAsync: Failed to assign driver {DriverId} to cluster {ClusterId}.", driverId, clusterId);
        //        return new GeneralResponse<bool> { Success = false, Message = $"Failed to assign driver: {ex.Message}", Data = false };
        //    }
        //}
        public async Task AutoAssignDriverAsync(Delivery delivery, string clusterId, double? overrideLat = null, double? overrideLon = null)
        {
            //if (delivery == null || string.IsNullOrWhiteSpace(clusterId))
            //{
            //    _logger.LogWarning("AutoAssignDriverAsync: Invalid input - delivery or clusterId is null/empty.");
            //    throw new ArgumentNullException("Delivery and cluster ID are required.");
            //}

            //try
            //{
            //    var clusterResult = await _clusterService.GetByIdAsync(clusterId);
            //    if (!clusterResult.Success || clusterResult.Data == null)
            //    {
            //        _logger.LogWarning("AutoAssignDriverAsync: Cluster {ClusterId} not found.", clusterId);
            //        throw new InvalidOperationException("Cluster not found.");
            //    }

            //    var clusterDto = clusterResult.Data;

            //    double locationLat, locationLon;
            //    if (overrideLat.HasValue && overrideLon.HasValue)
            //    {
            //        locationLat = overrideLat.Value;
            //        locationLon = overrideLon.Value;
            //    }
            //    else
            //    {
            //        if (!string.IsNullOrWhiteSpace(clusterDto.TechCompanyId))
            //        {
            //            var techCompany = await _techCompanyRepo.GetByIdAsync(clusterDto.TechCompanyId);
            //            if (techCompany == null || !techCompany.User.Latitude.HasValue || !techCompany.User.Longitude.HasValue)
            //            {
            //                _logger.LogWarning("AutoAssignDriverAsync: Tech company {TechCompanyId} coordinates missing for cluster {ClusterId}.", clusterDto.TechCompanyId, clusterId);
            //                throw new InvalidOperationException("Tech company coordinates are missing.");
            //            }
            //            locationLat = techCompany.User.Latitude.Value;
            //            locationLon = techCompany.User.Longitude.Value;
            //        }
            //        else if (clusterDto.DropoffLatitude.HasValue && clusterDto.DropoffLongitude.HasValue)
            //        {
            //            locationLat = clusterDto.DropoffLatitude.Value;
            //            locationLon = clusterDto.DropoffLongitude.Value;
            //        }
            //        else
            //        {
            //            _logger.LogWarning("AutoAssignDriverAsync: Unable to determine location for cluster {ClusterId}.", clusterId);
            //            throw new InvalidOperationException("Unable to determine location for cluster.");
            //        }
            //    }

            //    var response = await _deliveryPersonService.GetAvailableDeliveryPersonsAsync();
            //    var availableDrivers = response.Success ? response.Data?.ToList() ?? new List<DeliveryPersonReadDTO>() : new List<DeliveryPersonReadDTO>();

            //    if (!availableDrivers.Any())
            //    {
            //        _logger.LogWarning("AutoAssignDriverAsync: No available drivers for cluster {ClusterId}.", clusterId);
            //        throw new InvalidOperationException("No available drivers found.");
            //    }

            //    var candidates = availableDrivers
            //        .Where(d => d.CurrentLatitude.HasValue && d.CurrentLongitude.HasValue)
            //        .Select(d => new
            //        {
            //            Driver = d,
            //            DistanceKm = _locationService.CalculateDistance(locationLat, locationLon, d.CurrentLatitude.Value, d.CurrentLongitude.Value)
            //        })
            //        .OrderBy(x => x.DistanceKm)
            //        .ToList();

            //    if (!candidates.Any())
            //    {
            //        _logger.LogWarning("AutoAssignDriverAsync: No drivers with location data for cluster {ClusterId}.", clusterId);
            //        throw new InvalidOperationException("No available drivers with location data found.");
            //    }

            //    var best = candidates.First();
            //    if (_settings.MaxDriverDistanceKm > 0 && best.DistanceKm > _settings.MaxDriverDistanceKm)
            //    {
            //        _logger.LogWarning("AutoAssignDriverAsync: Nearest driver {DistanceKm:F1} km exceeds max {MaxDistanceKm} km for cluster {ClusterId}.", best.DistanceKm, _settings.MaxDriverDistanceKm, clusterId);
            //        throw new InvalidOperationException($"Nearest driver {best.DistanceKm:F1} km is beyond allowed max distance ({_settings.MaxDriverDistanceKm} km).");
            //    }

            //    var nearestDriver = best.Driver;
            //    var assignResult = await _clusterService.AssignDriverAsync(clusterId, nearestDriver.Id);
            //    if (!assignResult.Success)
            //    {
            //        _logger.LogError("AutoAssignDriverAsync: Failed to assign driver {DriverId} to cluster {ClusterId}: {Message}", nearestDriver.Id, clusterId, assignResult.Message);
            //        throw new InvalidOperationException($"Failed to assign driver to cluster: {assignResult.Message}");
            //    }

            //    delivery.DeliveryPersonId = nearestDriver.Id;
            //    delivery.Status = DeliveryStatus.Assigned;
            //    _deliveryRepo.Update(delivery);

            //    var offer = new DeliveryOffer
            //    {
            //        Id = Guid.NewGuid().ToString(),
            //        DeliveryId = delivery.Id,
            //        ClusterId = clusterId,
            //        DeliveryPersonId = nearestDriver.Id,
            //        Status = DeliveryOfferStatus.Pending,
            //        CreatedAt = DateTime.UtcNow,
            //        ExpiryTime = DateTime.UtcNow.Add(_settings.OfferExpiryTime),
            //        IsActive = true
            //    };

            //    await _deliveryOfferRepo.AddAsync(offer);
            //    await _deliveryOfferRepo.SaveChangesAsync();
            //    await _deliveryRepo.SaveChangesAsync();

            //    var notifyUserId = nearestDriver.UserId ?? nearestDriver.Id;
            //    await _notificationService.SendNotificationAsync(
            //        notifyUserId,
            //        $"New delivery offer: #{delivery.TrackingNumber ?? delivery.Id} — {best.DistanceKm:F2} km from you",
            //        NotificationType.DeliveryOfferCreated,
            //        delivery.Id,
            //        "Delivery");

            //    _logger.LogInformation("AutoAssignDriverAsync: Driver {DriverId} assigned to cluster {ClusterId} for delivery {DeliveryId}.", nearestDriver.Id, clusterId, delivery.Id);
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "AutoAssignDriverAsync: Failed for cluster {ClusterId} in delivery {DeliveryId}.", clusterId, delivery?.Id);
            //    throw;
            //}


            if (delivery == null || string.IsNullOrWhiteSpace(clusterId))
                throw new ArgumentNullException("Delivery and cluster ID are required.");

            try
            {
                // Get the cluster
                var clusterResult = await _clusterService.GetByIdAsync(clusterId);
                if (!clusterResult.Success || clusterResult.Data == null)
                    throw new InvalidOperationException("Cluster not found.");
                var clusterDto = clusterResult.Data;

                // Determine pickup location
                double locationLat, locationLon;
                if (overrideLat.HasValue && overrideLon.HasValue)
                {
                    locationLat = overrideLat.Value;
                    locationLon = overrideLon.Value;
                }
                else if (!string.IsNullOrWhiteSpace(clusterDto.TechCompanyId))
                {
                    var techCompany = await _techCompanyRepo.GetByIdAsync(clusterDto.TechCompanyId);
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

                // Get available drivers
                var response = await _deliveryPersonService.GetAvailableDeliveryPersonsAsync();
                var availableDrivers = response.Success ? response.Data?.ToList() ?? new List<DeliveryPersonReadDTO>() : new List<DeliveryPersonReadDTO>();
                if (!availableDrivers.Any())
                    throw new InvalidOperationException("No available drivers found.");

                // Filter and sort by distance
                var candidates = availableDrivers
                    .Where(d => d.Latitude.HasValue && d.Longitude.HasValue)
                    .Select(d => new
                    {
                        Driver = d,
                        DistanceKm = _locationService.CalculateDistance(locationLat, locationLon, d.Latitude.Value, d.Longitude.Value)
                    })
                    .OrderBy(x => x.DistanceKm)
                    .ToList();

                if (!candidates.Any())
                    throw new InvalidOperationException("No drivers with location data found.");

                // Take top 10 drivers
                var topDrivers = candidates.Take(10).ToList();

                foreach (var candidate in topDrivers)
                {
                    // Create DeliveryOffer
                    var offer = new DeliveryOffer
                    {
                        Id = Guid.NewGuid().ToString(),
                        DeliveryId = delivery.Id,
                        ClusterId = clusterId,
                        DeliveryPersonId = candidate.Driver.Id,
                        Status = DeliveryOfferStatus.Pending,
                        CreatedAt = DateTime.Now,
                        ExpiryTime = DateTime.Now.Add(_settings.OfferExpiryTime),
                        IsActive = true
                    };

                    await _deliveryOfferRepo.AddAsync(offer);

                    // Send notification
                    var notifyUserId = candidate.Driver.UserId ?? candidate.Driver.Id;
                    await _notificationService.SendNotificationAsync(
                        notifyUserId,
                        NotificationType.DeliveryOfferCreated,
                        delivery.Id,
                        "Delivery",
                        delivery.TrackingNumber ?? delivery.Id,
                        $"{candidate.DistanceKm:F2} km from you"
                    );
                }

                await _deliveryOfferRepo.SaveChangesAsync();

                _logger.LogInformation(
                    "AutoAssignDriverAsync: Created {OfferCount} delivery offers for cluster {ClusterId} in delivery {DeliveryId}.",
                    topDrivers.Count, clusterId, delivery.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AutoAssignDriverAsync: Failed for cluster {ClusterId} in delivery {DeliveryId}.", clusterId, delivery?.Id);
            }
        }

        public async Task<GeneralResponse<bool>> AssignDriverToClusterAsync(string clusterId, string driverId)
        {
            if (string.IsNullOrWhiteSpace(clusterId) || string.IsNullOrWhiteSpace(driverId))
                return new GeneralResponse<bool> { Success = false, Message = "Cluster ID and Driver ID are required." };

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var clusterResult = await _clusterService.GetByIdAsync(clusterId);
                if (!clusterResult.Success || clusterResult.Data == null)
                    return new GeneralResponse<bool> { Success = false, Message = "Cluster not found." };

                var cluster = clusterResult.Data;

                var delivery = await _deliveryRepo.GetByIdAsync(cluster.DeliveryId);
                if (delivery == null)
                    return new GeneralResponse<bool> { Success = false, Message = "Delivery not found." };

                // Get the offer for this driver
                var offer = await _deliveryOfferRepo.GetFirstOrDefaultAsync(o =>
                    o.ClusterId == clusterId && o.DeliveryPersonId == driverId && o.Status == DeliveryOfferStatus.Pending);

                if (offer == null)
                    return new GeneralResponse<bool> { Success = false, Message = "Offer not found or already accepted/expired." };

                // Accept this offer
                offer.Status = DeliveryOfferStatus.Accepted;
                offer.IsActive = false;
                _deliveryOfferRepo.Update(offer);

                // Expire all other offers for this cluster
                var otherOffers = await _deliveryOfferRepo.FindAsync(o =>
                    o.ClusterId == clusterId && o.Id != offer.Id && o.Status == DeliveryOfferStatus.Pending);
                foreach (var o in otherOffers)
                {
                    o.Status = DeliveryOfferStatus.Expired;
                    o.IsActive = false;
                    _deliveryOfferRepo.Update(o);
                }

                // Assign driver to cluster
                var assignResult = await _clusterService.AssignDriverAsync(clusterId, driverId);
                if (!assignResult.Success)
                    return new GeneralResponse<bool> { Success = false, Message = assignResult.Message };

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

                _logger.LogInformation("AssignDriverToClusterAsync: Driver {DriverId} accepted cluster {ClusterId}.", driverId, clusterId);
                return new GeneralResponse<bool> { Success = true, Message = "Driver assigned successfully.", Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AssignDriverToClusterAsync: Failed to assign driver {DriverId} to cluster {ClusterId}.", driverId, clusterId);
                return new GeneralResponse<bool> { Success = false, Message = $"Failed to assign driver: {ex.Message}", Data = false };
            }
        }

        //public async Task<GeneralResponse<bool>> AcceptDeliveryAsync(string clusterId, string driverId)
        //{
        //    if (string.IsNullOrWhiteSpace(clusterId) || string.IsNullOrWhiteSpace(driverId))
        //    {
        //        _logger.LogWarning("AcceptDeliveryAsync: Invalid input - clusterId or driverId is empty.");
        //        return new GeneralResponse<bool> { Success = false, Message = "Cluster ID and Driver ID are required." };
        //    }

        //    using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        //    try
        //    {
        //        var clusterResult = await _clusterService.GetByIdAsync(clusterId);
        //        if (!clusterResult.Success || clusterResult.Data == null)
        //        {
        //            _logger.LogWarning("AcceptDeliveryAsync: Cluster {ClusterId} not found.", clusterId);
        //            return new GeneralResponse<bool> { Success = false, Message = "Cluster not found." };
        //        }

        //        var cluster = clusterResult.Data;
        //        if (cluster.AssignedDriverId != driverId)
        //        {
        //            _logger.LogWarning("AcceptDeliveryAsync: Driver {DriverId} not assigned to cluster {ClusterId}.", driverId, clusterId);
        //            return new GeneralResponse<bool> { Success = false, Message = "Driver not assigned to this cluster." };
        //        }

        //        var delivery = await _deliveryRepo.GetByIdAsync(cluster.DeliveryId);
        //        if (delivery == null)
        //        {
        //            _logger.LogWarning("AcceptDeliveryAsync: Delivery {DeliveryId} not found for cluster {ClusterId}.", cluster.DeliveryId, clusterId);
        //            return new GeneralResponse<bool> { Success = false, Message = "Delivery not found." };
        //        }

        //        var offer = (await _deliveryOfferRepo.GetAllAsync()).FirstOrDefault(o => o.ClusterId == clusterId && o.DeliveryPersonId == driverId && o.IsActive);
        //        if (offer == null)
        //        {
        //            _logger.LogWarning("AcceptDeliveryAsync: No active offer found for driver {DriverId} in cluster {ClusterId}.", driverId, clusterId);
        //            return new GeneralResponse<bool> { Success = false, Message = "No active offer found." };
        //        }

        //        offer.Status = DeliveryOfferStatus.Accepted;
        //        offer.RespondedAt = DateTime.UtcNow;
        //        offer.IsActive = false;
        //        _deliveryOfferRepo.Update(offer);

        //        // Get driver location
        //        var driverResponse = await _deliveryPersonService.GetByIdAsync(driverId);
        //        if (!driverResponse.Success || driverResponse.Data == null)
        //        {
        //            _logger.LogWarning("AcceptDeliveryAsync: Driver {DriverId} not found.", driverId);
        //            return new GeneralResponse<bool> { Success = false, Message = "Driver not found." };
        //        }
        //        var driverDto = driverResponse.Data;
        //        var driverLat = driverDto.CurrentLatitude ?? throw new InvalidOperationException("Driver location missing.");
        //        var driverLon = driverDto.CurrentLongitude ?? throw new InvalidOperationException("Driver location missing.");

        //        bool isSplit = false;
        //        string newClusterId = clusterId;

        //        if (!string.IsNullOrWhiteSpace(cluster.TechCompanyId))
        //        {
        //            var techCompany = await _techCompanyRepo.GetByIdAsync(cluster.TechCompanyId);
        //            if (techCompany == null || !techCompany.User.Latitude.HasValue || !techCompany.User.Longitude.HasValue)
        //            {
        //                _logger.LogWarning("AcceptDeliveryAsync: Tech company {TechCompanyId} location missing for cluster {ClusterId}.", cluster.TechCompanyId, clusterId);
        //                return new GeneralResponse<bool> { Success = false, Message = "Tech company location missing." };
        //            }
        //            var companyLat = techCompany.User.Latitude.Value;
        //            var companyLon = techCompany.User.Longitude.Value;

        //            var distanceToCompany = _locationService.CalculateDistance(driverLat, driverLon, companyLat, companyLon);

        //            if (distanceToCompany > _settings.MaxDriverDistanceKm)
        //            {
        //                isSplit = true;
        //                var customerLat = cluster.DropoffLatitude ?? delivery.DropoffLatitude ?? throw new InvalidOperationException("Customer location missing.");
        //                var customerLon = cluster.DropoffLongitude ?? delivery.DropoffLongitude ?? throw new InvalidOperationException("Customer location missing.");

        //                var (handoverLat, handoverLon) = _locationService.GetMidpoint(companyLat, companyLon, customerLat, customerLon);

        //                // Create pickup leg (company to handover)
        //                var pickupClusterDto = new DeliveryClusterCreateDTO
        //                {
        //                    DeliveryId = delivery.Id,
        //                    TechCompanyId = cluster.TechCompanyId,
        //                    TechCompanyName = cluster.TechCompanyName,
        //                    DistanceKm = _locationService.CalculateDistance(companyLat, companyLon, handoverLat, handoverLon),
        //                    Price = cluster.Price / 2,  // Adjust pricing logic as needed
        //                    DropoffLatitude = handoverLat,
        //                    DropoffLongitude = handoverLon,
        //                    SequenceOrder = cluster.SequenceOrder
        //                };
        //                var pickupResult = await _clusterService.CreateClusterAsync(delivery.Id, pickupClusterDto);
        //                if (!pickupResult.Success)
        //                {
        //                    _logger.LogError("AcceptDeliveryAsync: Failed to create pickup cluster: {Message}", pickupResult.Message);
        //                    return new GeneralResponse<bool> { Success = false, Message = pickupResult.Message };
        //                }

        //                // Auto-assign pickup leg nearest to company (no override, uses company location)
        //                await AutoAssignDriverAsync(delivery, pickupResult.Data.Id);

        //                // Create delivery leg (handover to customer, assign original driver)
        //                var deliveryLegDto = new DeliveryClusterCreateDTO
        //                {
        //                    DeliveryId = delivery.Id,
        //                    DistanceKm = _locationService.CalculateDistance(handoverLat, handoverLon, customerLat, customerLon),
        //                    Price = cluster.Price / 2,
        //                    PickupLatitude = handoverLat,
        //                    PickupLongitude = handoverLon,
        //                    DropoffLatitude = customerLat,
        //                    DropoffLongitude = customerLon,
        //                    SequenceOrder = cluster.SequenceOrder + 1,
        //                    AssignedDriverId = driverId
        //                };
        //                var deliveryLegResult = await _clusterService.CreateClusterAsync(delivery.Id, deliveryLegDto);
        //                if (!deliveryLegResult.Success)
        //                {
        //                    _logger.LogError("AcceptDeliveryAsync: Failed to create delivery leg cluster: {Message}", deliveryLegResult.Message);
        //                    return new GeneralResponse<bool> { Success = false, Message = deliveryLegResult.Message };
        //                }

        //                newClusterId = deliveryLegResult.Data.Id;

        //                // Update offer to new delivery leg
        //                offer.ClusterId = newClusterId;
        //                _deliveryOfferRepo.Update(offer);

        //                // Delete original cluster
        //                var deleteResult = await _clusterService.DeleteClusterAsync(clusterId);
        //                if (!deleteResult.Success)
        //                {
        //                    _logger.LogError("AcceptDeliveryAsync: Failed to delete original cluster {ClusterId}: {Message}", clusterId, deleteResult.Message);
        //                    return new GeneralResponse<bool> { Success = false, Message = deleteResult.Message };
        //                }

        //                await _notificationService.SendNotificationAsync(
        //                    driverId,
        //                    $"Delivery updated for #{delivery.TrackingNumber ?? delivery.Id} due to distance split.",
        //                    NotificationType.DeliveryAssigned,
        //                    delivery.Id,
        //                    "Delivery");
        //            }
        //        }

        //        // Update statuses
        //        delivery.Status = DeliveryStatus.Assigned;
        //        _deliveryRepo.Update(delivery);

        //        var updatedClusterDto = new DeliveryClusterDTO
        //        {
        //            Id = newClusterId,
        //            DeliveryId = delivery.Id,
        //            TechCompanyId = cluster.TechCompanyId,
        //            TechCompanyName = cluster.TechCompanyName,
        //            DistanceKm = cluster.DistanceKm,
        //            Price = cluster.Price,
        //            Status = DeliveryClusterStatus.Assigned,
        //            AssignedDriverId = driverId,
        //            AssignedDriverName = cluster.AssignedDriverName,
        //            AssignmentTime = DateTime.UtcNow,
        //            DropoffLatitude = cluster.DropoffLatitude,
        //            DropoffLongitude = cluster.DropoffLongitude,
        //            SequenceOrder = cluster.SequenceOrder,
        //            DriverOfferCount = cluster.DriverOfferCount,
        //            Latitude = cluster.Latitude,
        //            Longitude = cluster.Longitude,
        //            EstimatedDistance = cluster.EstimatedDistance,
        //            EstimatedPrice = cluster.EstimatedPrice,
        //            PickupLatitude = cluster.PickupLatitude,
        //            PickupLongitude = cluster.PickupLongitude
        //        };
        //        var clusterUpdate = await _clusterService.UpdateClusterAsync(newClusterId, updatedClusterDto);
        //        if (!clusterUpdate.Success)
        //        {
        //            _logger.LogError("AcceptDeliveryAsync: Failed to update cluster {ClusterId}: {Message}", newClusterId, clusterUpdate.Message);
        //            return new GeneralResponse<bool> { Success = false, Message = clusterUpdate.Message };
        //        }

        //        await _deliveryRepo.SaveChangesAsync();
        //        await _deliveryOfferRepo.SaveChangesAsync();

        //        await _notificationService.SendNotificationAsync(
        //            driverId,
        //            $"Delivery offer accepted: #{delivery.TrackingNumber ?? delivery.Id}.",
        //            NotificationType.DeliveryOfferAccepted,
        //            delivery.Id,
        //            "Delivery");

        //        scope.Complete();
        //        _logger.LogInformation("AcceptDeliveryAsync: Driver {DriverId} accepted cluster {ClusterId} (split: {IsSplit}).", driverId, clusterId, isSplit);
        //        return new GeneralResponse<bool> { Success = true, Message = "Delivery offer accepted.", Data = true };
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "AcceptDeliveryAsync: Failed for driver {DriverId} on cluster {ClusterId}.", driverId, clusterId);
        //        return new GeneralResponse<bool> { Success = false, Message = $"Failed to accept delivery: {ex.Message}", Data = false };
        //    }
        //}

        public async Task<GeneralResponse<bool>> AcceptDeliveryAsync(string clusterId, string driverId)
        {
            if (string.IsNullOrWhiteSpace(clusterId) || string.IsNullOrWhiteSpace(driverId))
                return new GeneralResponse<bool> { Success = false, Message = "Cluster ID and Driver ID are required." };

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                var clusterResult = await _clusterService.GetByIdAsync(clusterId);
                if (!clusterResult.Success || clusterResult.Data == null)
                    return new GeneralResponse<bool> { Success = false, Message = "Cluster not found." };

                var cluster = clusterResult.Data;

                if (cluster.AssignedDriverId != driverId)
                    return new GeneralResponse<bool> { Success = false, Message = "Driver not assigned to this cluster." };

                var delivery = await _deliveryRepo.GetByIdAsync(cluster.DeliveryId);
                if (delivery == null)
                    return new GeneralResponse<bool> { Success = false, Message = "Delivery not found." };

                var offer = (await _deliveryOfferRepo.FindAsync(
                    o => o.ClusterId == clusterId && o.DeliveryPersonId == driverId && o.IsActive
                )).FirstOrDefault();

                if (offer == null)
                    return new GeneralResponse<bool> { Success = false, Message = "No active offer found." };

                offer.Status = DeliveryOfferStatus.Accepted;
                offer.RespondedAt = DateTime.Now;
                offer.IsActive = false;
                _deliveryOfferRepo.Update(offer);

                var driver = (await _deliveryPersonService.GetByIdAsync(driverId)).Data
                             ?? throw new InvalidOperationException("Driver not found");

                bool isSplit = false;
                if (!string.IsNullOrWhiteSpace(cluster.TechCompanyId))
                {
                    var techCompany = await _techCompanyRepo.GetByIdAsync(cluster.TechCompanyId);
                    var distance = _locationService.CalculateDistance(driver.CurrentLatitude.Value, driver.CurrentLongitude.Value,
                                                                      techCompany.User.Latitude.Value, techCompany.User.Longitude.Value);

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

                await Task.WhenAll(_deliveryRepo.SaveChangesAsync(), _deliveryOfferRepo.SaveChangesAsync());
                await _notificationService.SendNotificationAsync(
                    driverId,
                    NotificationType.DeliveryOfferAccepted,
                    delivery.Id,
                    "Delivery",
                    delivery.TrackingNumber ?? delivery.Id
                );

                scope.Complete();
                return new GeneralResponse<bool> { Success = true, Message = "Delivery offer accepted.", Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AcceptDeliveryAsync failed for driver {DriverId} on cluster {ClusterId}.", driverId, clusterId);
                return new GeneralResponse<bool> { Success = false, Message = $"Failed to accept delivery: {ex.Message}", Data = false };
            }
        }

        //public async Task<GeneralResponse<bool>> DeclineDeliveryAsync(string clusterId, string driverId)
        //{
        //    if (string.IsNullOrWhiteSpace(clusterId) || string.IsNullOrWhiteSpace(driverId))
        //    {
        //        _logger.LogWarning("DeclineDeliveryAsync: Invalid input - clusterId or driverId is empty.");
        //        return new GeneralResponse<bool> { Success = false, Message = "Cluster ID and Driver ID are required." };
        //    }

        //    using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        //    try
        //    {
        //        var clusterResult = await _clusterService.GetByIdAsync(clusterId);
        //        if (!clusterResult.Success || clusterResult.Data == null)
        //        {
        //            _logger.LogWarning("DeclineDeliveryAsync: Cluster {ClusterId} not found.", clusterId);
        //            return new GeneralResponse<bool> { Success = false, Message = "Cluster not found." };
        //        }

        //        var cluster = clusterResult.Data;
        //        if (cluster.AssignedDriverId != driverId)
        //        {
        //            _logger.LogWarning("DeclineDeliveryAsync: Driver {DriverId} not assigned to cluster {ClusterId}.", driverId, clusterId);
        //            return new GeneralResponse<bool> { Success = false, Message = "Driver not assigned to this cluster." };
        //        }

        //        var delivery = await _deliveryRepo.GetByIdAsync(cluster.DeliveryId);
        //        if (delivery == null)
        //        {
        //            _logger.LogWarning("DeclineDeliveryAsync: Delivery {DeliveryId} not found for cluster {ClusterId}.", cluster.DeliveryId, clusterId);
        //            return new GeneralResponse<bool> { Success = false, Message = "Delivery not found." };
        //        }

        //        var offer = (await _deliveryOfferRepo.GetAllAsync()).FirstOrDefault(o => o.ClusterId == clusterId && o.DeliveryPersonId == driverId && o.IsActive);
        //        if (offer == null)
        //        {
        //            _logger.LogWarning("DeclineDeliveryAsync: No active offer found for driver {DriverId} in cluster {ClusterId}.", driverId, clusterId);
        //            return new GeneralResponse<bool> { Success = false, Message = "No active offer found." };
        //        }

        //        offer.Status = DeliveryOfferStatus.Declined;
        //        offer.RespondedAt = DateTime.UtcNow;
        //        offer.IsActive = false;
        //        _deliveryOfferRepo.Update(offer);

        //        // Clear assignment
        //        var clusterUpdateDto = new DeliveryClusterDTO
        //        {
        //            Id = cluster.Id,
        //            DeliveryId = cluster.DeliveryId,
        //            TechCompanyId = cluster.TechCompanyId,
        //            TechCompanyName = cluster.TechCompanyName,
        //            DistanceKm = cluster.DistanceKm,
        //            Price = cluster.Price,
        //            Status = DeliveryClusterStatus.Pending,
        //            AssignedDriverId = null,
        //            AssignedDriverName = null,
        //            AssignmentTime = null,
        //            DropoffLatitude = cluster.DropoffLatitude,
        //            DropoffLongitude = cluster.DropoffLongitude,
        //            SequenceOrder = cluster.SequenceOrder,
        //            DriverOfferCount = cluster.DriverOfferCount + 1,
        //            Latitude = cluster.Latitude,
        //            Longitude = cluster.Longitude,
        //            EstimatedDistance = cluster.EstimatedDistance,
        //            EstimatedPrice = cluster.EstimatedPrice,
        //            PickupLatitude = cluster.PickupLatitude,
        //            PickupLongitude = cluster.PickupLongitude
        //        };
        //        var clusterUpdate = await _clusterService.UpdateClusterAsync(clusterId, clusterUpdateDto);
        //        if (!clusterUpdate.Success)
        //        {
        //            _logger.LogError("DeclineDeliveryAsync: Failed to update cluster {ClusterId}: {Message}", clusterId, clusterUpdate.Message);
        //            return new GeneralResponse<bool> { Success = false, Message = clusterUpdate.Message };
        //        }

        //        delivery.DeliveryPersonId = null;
        //        delivery.Status = DeliveryStatus.Pending;
        //        _deliveryRepo.Update(delivery);

        //        await _deliveryOfferRepo.SaveChangesAsync();
        //        await _deliveryRepo.SaveChangesAsync();

        //        // Re-assign to next nearest if retries allow
        //        if (delivery.RetryCount < _settings.MaxRetries)
        //        {
        //            delivery.RetryCount++;
        //            _deliveryRepo.Update(delivery);
        //            await _deliveryRepo.SaveChangesAsync();

        //            // Use customer location for re-assignment
        //            await AutoAssignDriverAsync(delivery, clusterId, delivery.DropoffLatitude.Value, delivery.DropoffLongitude.Value);
        //        }

        //        await _notificationService.SendNotificationAsync(
        //            driverId,
        //            $"Delivery offer declined: #{delivery.TrackingNumber ?? delivery.Id}.",
        //            NotificationType.DeliveryOfferExpired,
        //            delivery.Id,
        //            "Delivery");

        //        scope.Complete();
        //        _logger.LogInformation("DeclineDeliveryAsync: Driver {DriverId} declined cluster {ClusterId}.", driverId, clusterId);
        //        return new GeneralResponse<bool> { Success = true, Message = "Delivery offer declined.", Data = true };
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "DeclineDeliveryAsync: Failed for driver {DriverId} on cluster {ClusterId}.", driverId, clusterId);
        //        return new GeneralResponse<bool> { Success = false, Message = $"Failed to decline delivery: {ex.Message}", Data = false };
        //    }
        //}

        //public async Task<GeneralResponse<bool>> CancelDeliveryAsync(string deliveryId)
        //{
        //    if (string.IsNullOrWhiteSpace(deliveryId))
        //    {
        //        _logger.LogWarning("CancelDeliveryAsync: Invalid input - deliveryId is empty.");
        //        return new GeneralResponse<bool> { Success = false, Message = "Delivery ID is required." };
        //    }

        //    using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        //    try
        //    {
        //        var delivery = await _deliveryRepo.GetByIdAsync(deliveryId);
        //        if (delivery == null)
        //        {
        //            _logger.LogWarning("CancelDeliveryAsync: Delivery {DeliveryId} not found.", deliveryId);
        //            return new GeneralResponse<bool> { Success = false, Message = "Delivery not found." };
        //        }

        //        if (delivery.Status == DeliveryStatus.Delivered || delivery.Status == DeliveryStatus.Cancelled)
        //        {
        //            _logger.LogWarning("CancelDeliveryAsync: Delivery {DeliveryId} is already {Status}.", deliveryId, delivery.Status);
        //            return new GeneralResponse<bool> { Success = false, Message = $"Delivery is already {delivery.Status}." };
        //        }

        //        delivery.Status = DeliveryStatus.Cancelled;
        //        delivery.UpdatedAt = DateTime.UtcNow;
        //        _deliveryRepo.Update(delivery);

        //        var clustersResult = await _clusterService.GetByDeliveryIdAsync(deliveryId);
        //        if (clustersResult.Success && clustersResult.Data != null)
        //        {
        //            foreach (var cluster in clustersResult.Data)
        //            {
        //                var clusterUpdateDto = new DeliveryClusterDTO
        //                {
        //                    Id = cluster.Id,
        //                    DeliveryId = cluster.DeliveryId,
        //                    TechCompanyId = cluster.TechCompanyId,
        //                    TechCompanyName = cluster.TechCompanyName,
        //                    DistanceKm = cluster.DistanceKm,
        //                    Price = cluster.Price,
        //                    Status = DeliveryClusterStatus.Cancelled,
        //                    AssignedDriverId = null,
        //                    AssignedDriverName = null,
        //                    AssignmentTime = null,
        //                    DropoffLatitude = cluster.DropoffLatitude,
        //                    DropoffLongitude = cluster.DropoffLongitude,
        //                    SequenceOrder = cluster.SequenceOrder,
        //                    DriverOfferCount = cluster.DriverOfferCount,
        //                    Latitude = cluster.Latitude,
        //                    Longitude = cluster.Longitude,
        //                    EstimatedDistance = cluster.EstimatedDistance,
        //                    EstimatedPrice = cluster.EstimatedPrice,
        //                    PickupLatitude = cluster.PickupLatitude,
        //                    PickupLongitude = cluster.PickupLongitude
        //                };
        //                var clusterUpdate = await _clusterService.UpdateClusterAsync(cluster.Id, clusterUpdateDto);
        //                if (!clusterUpdate.Success)
        //                {
        //                    _logger.LogError("CancelDeliveryAsync: Failed to cancel cluster {ClusterId}: {Message}", cluster.Id, clusterUpdate.Message);
        //                    return new GeneralResponse<bool> { Success = false, Message = $"Failed to cancel cluster: {clusterUpdate.Message}" };
        //                }
        //            }
        //        }

        //        var offers = (await _deliveryOfferRepo.GetAllAsync()).Where(o => o.DeliveryId == deliveryId && o.IsActive);
        //        foreach (var offer in offers)
        //        {
        //            offer.Status = DeliveryOfferStatus.Expired;
        //            offer.IsActive = false;
        //            offer.RespondedAt = DateTime.UtcNow;
        //            _deliveryOfferRepo.Update(offer);
        //        }

        //        await _deliveryOfferRepo.SaveChangesAsync();
        //        await _deliveryRepo.SaveChangesAsync();

        //        await _notificationService.SendNotificationAsync(
        //            delivery.DeliveryPersonId ?? delivery.CustomerId ?? "admin",
        //            $"Delivery cancelled: #{delivery.TrackingNumber ?? delivery.Id}.",
        //            NotificationType.DeliveryCompleted,
        //            delivery.Id,
        //            "Delivery");

        //        scope.Complete();
        //        _logger.LogInformation("CancelDeliveryAsync: Delivery {DeliveryId} cancelled successfully.", deliveryId);
        //        return new GeneralResponse<bool> { Success = true, Message = "Delivery cancelled successfully.", Data = true };
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "CancelDeliveryAsync: Failed to cancel delivery {DeliveryId}.", deliveryId);
        //        return new GeneralResponse<bool> { Success = false, Message = $"Failed to cancel delivery: {ex.Message}", Data = false };
        //    }
        //}

        //public async Task<GeneralResponse<bool>> CompleteDeliveryAsync(string deliveryId, string driverId)
        //{
        //    if (string.IsNullOrWhiteSpace(deliveryId) || string.IsNullOrWhiteSpace(driverId))
        //    {
        //        _logger.LogWarning("CompleteDeliveryAsync: Invalid input - deliveryId or driverId is empty.");
        //        return new GeneralResponse<bool> { Success = false, Message = "Delivery ID and Driver ID are required." };
        //    }

        //    using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        //    try
        //    {
        //        var delivery = await _deliveryRepo.GetByIdAsync(deliveryId);
        //        if (delivery == null)
        //        {
        //            _logger.LogWarning("CompleteDeliveryAsync: Delivery {DeliveryId} not found.", deliveryId);
        //            return new GeneralResponse<bool> { Success = false, Message = "Delivery not found." };
        //        }

        //        if (delivery.DeliveryPersonId != driverId)
        //        {
        //            _logger.LogWarning("CompleteDeliveryAsync: Driver {DriverId} not assigned to delivery {DeliveryId}.", driverId, deliveryId);
        //            return new GeneralResponse<bool> { Success = false, Message = "Driver not assigned to this delivery." };
        //        }

        //        if (delivery.Status == DeliveryStatus.Delivered || delivery.Status == DeliveryStatus.Cancelled)
        //        {
        //            _logger.LogWarning("CompleteDeliveryAsync: Delivery {DeliveryId} is already {Status}.", deliveryId, delivery.Status);
        //            return new GeneralResponse<bool> { Success = false, Message = $"Delivery is already {delivery.Status}." };
        //        }

        //        delivery.Status = DeliveryStatus.Delivered;
        //        delivery.ActualDeliveryDate = DateTime.UtcNow;
        //        delivery.UpdatedAt = DateTime.UtcNow;
        //        _deliveryRepo.Update(delivery);

        //        var clustersResult = await _clusterService.GetByDeliveryIdAsync(deliveryId);
        //        if (clustersResult.Success && clustersResult.Data != null)
        //        {
        //            foreach (var cluster in clustersResult.Data)
        //            {
        //                var clusterUpdateDto = new DeliveryClusterDTO
        //                {
        //                    Id = cluster.Id,
        //                    DeliveryId = cluster.DeliveryId,
        //                    TechCompanyId = cluster.TechCompanyId,
        //                    TechCompanyName = cluster.TechCompanyName,
        //                    DistanceKm = cluster.DistanceKm,
        //                    Price = cluster.Price,
        //                    Status = DeliveryClusterStatus.Completed,
        //                    AssignedDriverId = cluster.AssignedDriverId,
        //                    AssignedDriverName = cluster.AssignedDriverName,
        //                    AssignmentTime = cluster.AssignmentTime,
        //                    DropoffLatitude = cluster.DropoffLatitude,
        //                    DropoffLongitude = cluster.DropoffLongitude,
        //                    SequenceOrder = cluster.SequenceOrder,
        //                    DriverOfferCount = cluster.DriverOfferCount,
        //                    Latitude = cluster.Latitude,
        //                    Longitude = cluster.Longitude,
        //                    EstimatedDistance = cluster.EstimatedDistance,
        //                    EstimatedPrice = cluster.EstimatedPrice,
        //                    PickupLatitude = cluster.PickupLatitude,
        //                    PickupLongitude = cluster.PickupLongitude
        //                };
        //                var clusterUpdate = await _clusterService.UpdateClusterAsync(cluster.Id, clusterUpdateDto);
        //                if (!clusterUpdate.Success)
        //                {
        //                    _logger.LogError("CompleteDeliveryAsync: Failed to complete cluster {ClusterId}: {Message}", cluster.Id, clusterUpdate.Message);
        //                    return new GeneralResponse<bool> { Success = false, Message = $"Failed to complete cluster: {clusterUpdate.Message}" };
        //                }
        //            }
        //        }

        //        var offers = (await _deliveryOfferRepo.GetAllAsync()).Where(o => o.DeliveryId == deliveryId && o.IsActive);
        //        foreach (var offer in offers)
        //        {
        //            offer.Status = DeliveryOfferStatus.Accepted;
        //            offer.IsActive = false;
        //            offer.RespondedAt = DateTime.UtcNow;
        //            _deliveryOfferRepo.Update(offer);
        //        }

        //        await _deliveryOfferRepo.SaveChangesAsync();
        //        await _deliveryRepo.SaveChangesAsync();

        //        await _notificationService.SendNotificationAsync(
        //            delivery.CustomerId ?? "admin",
        //            $"Delivery completed: #{delivery.TrackingNumber ?? delivery.Id}.",
        //            NotificationType.DeliveryCompleted,
        //            delivery.Id,
        //            "Delivery");

        //        scope.Complete();
        //        _logger.LogInformation("CompleteDeliveryAsync: Delivery {DeliveryId} completed by driver {DriverId}.", deliveryId, driverId);
        //        return new GeneralResponse<bool> { Success = true, Message = "Delivery completed successfully.", Data = true };
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "CompleteDeliveryAsync: Failed to complete delivery {DeliveryId} by driver {DriverId}.", deliveryId, driverId);
        //        return new GeneralResponse<bool> { Success = false, Message = $"Failed to complete delivery: {ex.Message}", Data = false };
        //    }
        //}



        // =========================
        // Helper methods
        // =========================
        
        private async Task<GeneralResponse<DeliveryClusterDTO>> UpdateClusterStatusAsync(string clusterId, DeliveryClusterStatus status, string? assignedDriverId = null)
        {
            var clusterResult = await _clusterService.GetByIdAsync(clusterId);
            if (!clusterResult.Success || clusterResult.Data == null)
                return new GeneralResponse<DeliveryClusterDTO> { Success = false, Message = "Cluster not found." };

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
                PickupLongitude = cluster.PickupLongitude
            };

            return await _clusterService.UpdateClusterAsync(clusterId, clusterDto);
        }

        public async Task<GeneralResponse<bool>> DeclineDeliveryAsync(string clusterId, string driverId)
        {
            if (string.IsNullOrWhiteSpace(clusterId) || string.IsNullOrWhiteSpace(driverId))
                return new GeneralResponse<bool> { Success = false, Message = "Cluster ID and Driver ID are required." };

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var clusterResult = await _clusterService.GetByIdAsync(clusterId);
                if (!clusterResult.Success || clusterResult.Data == null)
                    return new GeneralResponse<bool> { Success = false, Message = "Cluster not found." };

                var cluster = clusterResult.Data;
                if (cluster.AssignedDriverId != driverId)
                    return new GeneralResponse<bool> { Success = false, Message = "Driver not assigned to this cluster." };

                var delivery = await _deliveryRepo.GetByIdAsync(cluster.DeliveryId);
                if (delivery == null)
                    return new GeneralResponse<bool> { Success = false, Message = "Delivery not found." };

                var offers = await _deliveryOfferRepo.FindAsync(o => o.ClusterId == clusterId && o.DeliveryPersonId == driverId && o.IsActive);
                var offer = offers.FirstOrDefault();
                if (offer == null)
                    return new GeneralResponse<bool> { Success = false, Message = "No active offer found." };

                await UpdateOfferStatusAsync(offer, DeliveryOfferStatus.Declined);
                await UpdateClusterStatusAsync(clusterId, DeliveryClusterStatus.Pending, null);

                delivery.DeliveryPersonId = null;
                delivery.Status = DeliveryStatus.Pending;
                _deliveryRepo.Update(delivery);

                await _deliveryOfferRepo.SaveChangesAsync();
                await _deliveryRepo.SaveChangesAsync();

                // Retry logic
                if (delivery.RetryCount < _settings.MaxRetries)
                {
                    delivery.RetryCount++;
                    _deliveryRepo.Update(delivery);
                    await _deliveryRepo.SaveChangesAsync();

                    var lat = delivery.DropoffLatitude ?? throw new InvalidOperationException("Delivery dropoff latitude missing.");
                    var lon = delivery.DropoffLongitude ?? throw new InvalidOperationException("Delivery dropoff longitude missing.");
                    await AutoAssignDriverAsync(delivery, clusterId, lat, lon);
                }

                await _notificationService.SendNotificationAsync(
                 driverId,
                 NotificationType.DeliveryOfferExpired,
                 delivery.Id,
                 "Delivery",
                 delivery.TrackingNumber ?? delivery.Id
             );

                scope.Complete();
                return new GeneralResponse<bool> { Success = true, Message = "Delivery offer declined.", Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeclineDeliveryAsync failed for driver {DriverId} on cluster {ClusterId}.", driverId, clusterId);
                return new GeneralResponse<bool> { Success = false, Message = $"Failed to decline delivery: {ex.Message}", Data = false };
            }
        }

        public async Task<GeneralResponse<bool>> CancelDeliveryAsync(string deliveryId)
        {
            if (string.IsNullOrWhiteSpace(deliveryId))
                return new GeneralResponse<bool> { Success = false, Message = "Delivery ID is required." };

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var delivery = await _deliveryRepo.GetByIdAsync(deliveryId);
                if (delivery == null)
                    return new GeneralResponse<bool> { Success = false, Message = "Delivery not found." };

                if (delivery.Status == DeliveryStatus.Delivered || delivery.Status == DeliveryStatus.Cancelled)
                    return new GeneralResponse<bool> { Success = false, Message = $"Delivery is already {delivery.Status}." };

                delivery.Status = DeliveryStatus.Cancelled;
                delivery.UpdatedAt = DateTime.Now;
                _deliveryRepo.Update(delivery);

                var clustersResult = await _clusterService.GetByDeliveryIdAsync(deliveryId);
                if (clustersResult.Success && clustersResult.Data != null)
                {
                    foreach (var cluster in clustersResult.Data)
                        await UpdateClusterStatusAsync(cluster.Id, DeliveryClusterStatus.Cancelled);
                }

                var offers = (await _deliveryOfferRepo.GetAllAsync()).Where(o => o.DeliveryId == deliveryId && o.IsActive);
                foreach (var offer in offers)
                    await UpdateOfferStatusAsync(offer, DeliveryOfferStatus.Expired);

                await _deliveryOfferRepo.SaveChangesAsync();
                await _deliveryRepo.SaveChangesAsync();

                await _notificationService.SendNotificationAsync(
                    delivery.DeliveryPersonId ?? delivery.CustomerId ?? "Admin",
                    NotificationType.DeliveryCompleted,
                    delivery.Id,
                    "Delivery",
                    delivery.TrackingNumber ?? delivery.Id
                );

                scope.Complete();
                return new GeneralResponse<bool> { Success = true, Message = "Delivery cancelled successfully.", Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CancelDeliveryAsync failed for delivery {DeliveryId}.", deliveryId);
                return new GeneralResponse<bool> { Success = false, Message = $"Failed to cancel delivery: {ex.Message}", Data = false };
            }
        }

        public async Task<GeneralResponse<bool>> CompleteDeliveryAsync(string deliveryId, string driverId)
        {
            if (string.IsNullOrWhiteSpace(deliveryId) || string.IsNullOrWhiteSpace(driverId))
                return new GeneralResponse<bool> { Success = false, Message = "Delivery ID and Driver ID are required." };

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var delivery = await _deliveryRepo.GetByIdAsync(deliveryId);
                if (delivery == null)
                    return new GeneralResponse<bool> { Success = false, Message = "Delivery not found." };

                if (delivery.DeliveryPersonId != driverId)
                    return new GeneralResponse<bool> { Success = false, Message = "Driver not assigned to this delivery." };

                if (delivery.Status == DeliveryStatus.Delivered || delivery.Status == DeliveryStatus.Cancelled)
                    return new GeneralResponse<bool> { Success = false, Message = $"Delivery is already {delivery.Status}." };

                delivery.Status = DeliveryStatus.Delivered;
                delivery.ActualDeliveryDate = DateTime.Now;
                delivery.UpdatedAt = DateTime.Now;
                _deliveryRepo.Update(delivery);

                var clustersResult = await _clusterService.GetByDeliveryIdAsync(deliveryId);
                if (clustersResult.Success && clustersResult.Data != null)
                {
                    foreach (var cluster in clustersResult.Data)
                        await UpdateClusterStatusAsync(cluster.Id, DeliveryClusterStatus.Completed, cluster.AssignedDriverId);
                }

                var offers = (await _deliveryOfferRepo.GetAllAsync()).Where(o => o.DeliveryId == deliveryId && o.IsActive);
                foreach (var offer in offers)
                    await UpdateOfferStatusAsync(offer, DeliveryOfferStatus.Accepted);

                await _deliveryOfferRepo.SaveChangesAsync();
                await _deliveryRepo.SaveChangesAsync();

                await _notificationService.SendNotificationAsync(
                    delivery.CustomerId ?? "Admin",
                    NotificationType.DeliveryCompleted,
                    delivery.Id,
                    "Delivery",
                    delivery.TrackingNumber ?? delivery.Id
                );

                scope.Complete();
                return new GeneralResponse<bool> { Success = true, Message = "Delivery completed successfully.", Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CompleteDeliveryAsync failed for delivery {DeliveryId} by driver {DriverId}.", deliveryId, driverId);
                return new GeneralResponse<bool> { Success = false, Message = $"Failed to complete delivery: {ex.Message}", Data = false };
            }
        }

        public async Task<GeneralResponse<DeliveryReadDTO>> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("GetByIdAsync: Invalid input - id is empty.");
                return new GeneralResponse<DeliveryReadDTO> { Success = false, Message = "Delivery ID is required." };
            }

            try
            {
                var delivery = await _deliveryRepo.GetByIdAsync(id);
                if (delivery == null)
                {
                    _logger.LogWarning("GetByIdAsync: Delivery {DeliveryId} not found.", id);
                    return new GeneralResponse<DeliveryReadDTO> { Success = false, Message = "Delivery not found." };
                }

                var clustersResult = await _clusterService.GetByDeliveryIdAsync(id);
                var clusters = clustersResult.Success ? clustersResult.Data : Enumerable.Empty<DeliveryClusterDTO>();

                var readDto = DeliveryMapper.ToReadDTO(delivery, clusters);
                _logger.LogInformation("GetByIdAsync: Retrieved delivery {DeliveryId}.", id);
                return new GeneralResponse<DeliveryReadDTO> { Success = true, Message = "Delivery retrieved successfully.", Data = readDto };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync: Failed to retrieve delivery {DeliveryId}.", id);
                return new GeneralResponse<DeliveryReadDTO> { Success = false, Message = $"Failed to retrieve delivery: {ex.Message}" };
            }
        }

        public async Task<GeneralResponse<IEnumerable<DeliveryReadDTO>>> GetAllAsync()
        {
            try
            {
                var deliveries = await _deliveryRepo.GetAllAsync();
                var result = new List<DeliveryReadDTO>();

                foreach (var delivery in deliveries)
                {
                    var clustersResult = await _clusterService.GetByDeliveryIdAsync(delivery.Id);
                    var clusters = clustersResult.Success ? clustersResult.Data : Enumerable.Empty<DeliveryClusterDTO>();
                    result.Add(DeliveryMapper.ToReadDTO(delivery, clusters));
                }

                _logger.LogInformation("GetAllAsync: Retrieved {DeliveryCount} deliveries.", result.Count);
                return new GeneralResponse<IEnumerable<DeliveryReadDTO>>
                {
                    Success = true,
                    Message = "Deliveries retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllAsync: Failed to retrieve deliveries.");
                return new GeneralResponse<IEnumerable<DeliveryReadDTO>>
                {
                    Success = false,
                    Message = $"Failed to retrieve deliveries: {ex.Message}",
                    Data = Enumerable.Empty<DeliveryReadDTO>()
                };
            }
        }

        public async Task<GeneralResponse<bool>> DeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("DeleteAsync: Invalid input - id is empty.");
                return new GeneralResponse<bool> { Success = false, Message = "Delivery ID is required." };
            }

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var delivery = await _deliveryRepo.GetByIdAsync(id);
                if (delivery == null)
                {
                    _logger.LogWarning("DeleteAsync: Delivery {DeliveryId} not found.", id);
                    return new GeneralResponse<bool> { Success = false, Message = "Delivery not found." };
                }

                if (delivery.Status == DeliveryStatus.Delivered)
                {
                    _logger.LogWarning("DeleteAsync: Cannot delete delivered delivery {DeliveryId}.", id);
                    return new GeneralResponse<bool> { Success = false, Message = "Cannot delete a delivered delivery." };
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
                            _logger.LogError("DeleteAsync: Failed to delete cluster {ClusterId}: {Message}", cluster.Id, deleteResult.Message);
                            return new GeneralResponse<bool> { Success = false, Message = $"Failed to delete cluster: {deleteResult.Message}" };
                        }
                    }
                }

                var offers = (await _deliveryOfferRepo.GetAllAsync()).Where(o => o.DeliveryId == id && o.IsActive);
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
                _logger.LogInformation("DeleteAsync: Delivery {DeliveryId} deleted (cancelled) successfully.", id);
                return new GeneralResponse<bool> { Success = true, Message = "Delivery deleted successfully.", Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync: Failed to delete delivery {DeliveryId}.", id);
                return new GeneralResponse<bool> { Success = false, Message = $"Failed to delete delivery: {ex.Message}", Data = false };
            }
        }

        public async Task<GeneralResponse<IEnumerable<DeliveryPersonReadDTO>>> GetAvailableDeliveryPersonsAsync()
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
                        Data = new List<DeliveryPersonReadDTO>()
                    };

                var dtoList = drivers.Select(d => new DeliveryPersonReadDTO
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
                    CurrentLongitude = d.CurrentLongitude
                }).ToList();

                return new GeneralResponse<IEnumerable<DeliveryPersonReadDTO>>
                {
                    Success = true,
                    Data = dtoList
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get available delivery persons.");
                return new GeneralResponse<IEnumerable<DeliveryPersonReadDTO>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<DeliveryTrackingDTO>> GetDeliveryTrackingAsync(string deliveryId)
        {
            if (string.IsNullOrWhiteSpace(deliveryId))
            {
                _logger.LogWarning("GetDeliveryTrackingAsync: Invalid input - deliveryId is empty.");
                return new GeneralResponse<DeliveryTrackingDTO> { Success = false, Message = "Delivery ID is required." };
            }

            try
            {
                var delivery = await _deliveryRepo.GetByIdAsync(deliveryId);
                if (delivery == null)
                {
                    _logger.LogWarning("GetDeliveryTrackingAsync: Delivery {DeliveryId} not found.", deliveryId);
                    return new GeneralResponse<DeliveryTrackingDTO> { Success = false, Message = "Delivery not found." };
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
                    Clusters = clusterTracking
                };

                _logger.LogInformation("GetDeliveryTrackingAsync: Retrieved tracking for delivery {DeliveryId}.", deliveryId);
                return new GeneralResponse<DeliveryTrackingDTO>
                {
                    Success = true,
                    Message = "Tracking retrieved successfully.",
                    Data = trackingDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDeliveryTrackingAsync: Failed to retrieve tracking for delivery {DeliveryId}.", deliveryId);
                return new GeneralResponse<DeliveryTrackingDTO>
                {
                    Success = false,
                    Message = $"Failed to retrieve tracking: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<IEnumerable<Delivery>> GetDeliveriesWithExpiredOffersAsync()
        {
            return await _deliveryRepo.FindWithIncludesAsync(
                d => d.Status == DeliveryStatus.Pending &&
                     d.Offers.Any(o => o.IsActive && o.ExpiryTime <= DateTime.Now),
                d => d.Offers
            );
        }

        private async Task UpdateOfferStatusAsync(DeliveryOffer offer, DeliveryOfferStatus status)
        {
            if (offer == null) return;
            offer.Status = status;
            offer.RespondedAt = DateTime.Now;
            offer.IsActive = false;
            _deliveryOfferRepo.Update(offer);
        }
    }
}
