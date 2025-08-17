using Core.DTOs;
using Core.DTOs.DeliveryDTOs;
using Core.DTOs.DeliveryPersonDTOs;
using Core.Entities;
using Core.Enums;
using Core.Interfaces;
using Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Service
{
    public class DeliveryClusterService : IDeliveryClusterService
    {
        private readonly IRepository<DeliveryCluster> _clusterRepo;
        private readonly IRepository<Delivery> _deliveryRepo;
        private readonly IRepository<TechCompany> _techCompanyRepo;
        private readonly IDeliveryService _deliveryService;
        private readonly ILocationService _locationService;
        private readonly DeliveryAssignmentSettings _settings;
        private readonly INotificationService _notificationService;

        public DeliveryClusterService(
            IRepository<DeliveryCluster> clusterRepo,
            IRepository<Delivery> deliveryRepo,
            IRepository<TechCompany> techCompanyRepo,
            IDeliveryService deliveryService,
            ILocationService locationService,
            IOptions<DeliveryAssignmentSettings> settings,
            INotificationService notificationService)
        {
            _clusterRepo = clusterRepo ?? throw new ArgumentNullException(nameof(clusterRepo));
            _deliveryRepo = deliveryRepo ?? throw new ArgumentNullException(nameof(deliveryRepo));
            _techCompanyRepo = techCompanyRepo ?? throw new ArgumentNullException(nameof(techCompanyRepo));
            _deliveryService = deliveryService ?? throw new ArgumentNullException(nameof(deliveryService));
            _locationService = locationService ?? throw new ArgumentNullException(nameof(locationService));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }
        public async Task<GeneralResponse<DeliveryClusterDTO>> CreateClusterAsync(string deliveryId, DeliveryClusterCreateDTO dto)
        {
            if (dto == null)
                return new GeneralResponse<DeliveryClusterDTO>
                {
                    Success = false,
                    Message = "Cluster creation data is null."
                };

            var entity = DeliveryClusterMapper.ToEntity(dto);
            entity.DeliveryId = deliveryId;

            await _clusterRepo.AddAsync(entity);
            await _clusterRepo.SaveChangesAsync();

            var dtoResult = DeliveryClusterMapper.ToDTO(entity);

            await _notificationService.SendNotificationToRoleAsync(
            "Admin",
            NotificationType.SystemAlert,
            entity.Id,
            "DeliveryCluster",
            $"A new delivery cluster #{entity.Id} has been created for delivery #{deliveryId}."
            );

            if (!string.IsNullOrWhiteSpace(dto.AssignedDriverId))
            {
                await _notificationService.SendNotificationAsync(
                    dto.AssignedDriverId,
                    NotificationType.DeliveryAssigned,
                    entity.Id,
                    "DeliveryCluster",
                    $"You have been assigned to delivery cluster #{entity.Id} for delivery #{deliveryId}."
                );
            }

            return new GeneralResponse<DeliveryClusterDTO>
            {
                Success = true,
                Message = "Cluster created successfully.",
                Data = dtoResult
            };
        }

        public async Task<GeneralResponse<DeliveryClusterDTO>> GetByIdAsync(string clusterId)
        {
            var entity = await _clusterRepo.GetByIdWithIncludesAsync(
                clusterId,
                c => c.TechCompany,
                c => c.TechCompany.User,
                c => c.AssignedDriver,
                c => c.AssignedDriver.User,
                c => c.Tracking
            );

            if (entity == null)
                return new GeneralResponse<DeliveryClusterDTO> { Success = false, Message = "Cluster not found." };

            return new GeneralResponse<DeliveryClusterDTO>
            {
                Success = true,
                Data = DeliveryClusterMapper.ToDTO(entity)
            };
        }

        public async Task<GeneralResponse<IEnumerable<DeliveryClusterDTO>>> GetByDeliveryIdAsync(string deliveryId)
        {
            var clusters = await _clusterRepo.FindWithIncludesAsync(
                c => c.DeliveryId == deliveryId,
                c => c.TechCompany,
                c => c.TechCompany.User,
                c => c.AssignedDriver,
                c => c.AssignedDriver.User,
                c => c.Tracking
            );

            return new GeneralResponse<IEnumerable<DeliveryClusterDTO>>
            {
                Success = true,
                Data = clusters.Select(DeliveryClusterMapper.ToDTO)
            };
        }

        public async Task<GeneralResponse<DeliveryClusterDTO>> UpdateClusterAsync(string clusterId, DeliveryClusterDTO dto)
        {
            var entity = await _clusterRepo.GetByIdWithIncludesAsync(
                clusterId,
                c => c.TechCompany,
                c => c.TechCompany.User,
                c => c.AssignedDriver,
                c => c.AssignedDriver.User,
                c => c.Tracking
            );

            if (entity == null)
                return new GeneralResponse<DeliveryClusterDTO> { Success = false, Message = "Cluster not found." };

            // Store previous driver id for comparison
            var previousDriverId = entity.AssignedDriverId;

            DeliveryClusterMapper.UpdateEntity(entity, dto);
            _clusterRepo.Update(entity);
            await _clusterRepo.SaveChangesAsync();

            var dtoResult = DeliveryClusterMapper.ToDTO(entity);

            // Notify Admin about the update
            await _notificationService.SendNotificationToRoleAsync(
                "Admin",
                NotificationType.SystemAlert,
                entity.Id,
                "DeliveryCluster",
                $"Delivery cluster #{entity.Id} has been updated."
            );

            // Notify newly assigned driver if changed
            if (!string.IsNullOrWhiteSpace(entity.AssignedDriverId) && entity.AssignedDriverId != previousDriverId)
            {
                await _notificationService.SendNotificationAsync(
                    entity.AssignedDriverId,
                    NotificationType.DeliveryAssigned,
                    entity.Id,
                    "DeliveryCluster",
                    $"You have been assigned to delivery cluster #{entity.Id}."
                );
            }

            // Optionally notify TechCompany
            if (entity.TechCompany?.UserId != null)
            {
                await _notificationService.SendNotificationAsync(
                    entity.TechCompany.UserId,
                    NotificationType.SystemAlert,
                    $"Delivery cluster #{entity.Id} has been updated.",
                    entity.Id,
                    "DeliveryCluster"
                );
            }

            return new GeneralResponse<DeliveryClusterDTO>
            {
                Success = true,
                Message = "Cluster updated successfully.",
                Data = dtoResult
            };
        }

        public async Task<GeneralResponse<bool>> DeleteClusterAsync(string clusterId)
        {
            var entity = await _clusterRepo.GetByIdAsync(clusterId);
            if (entity == null)
                return new GeneralResponse<bool> { Success = false, Message = "Cluster not found." };

            _clusterRepo.Remove(entity);
            await _clusterRepo.SaveChangesAsync();

            return new GeneralResponse<bool>
            {
                Success = true,
                Message = "Cluster deleted successfully.",
                Data = true
            };
        }

        public async Task<GeneralResponse<DeliveryClusterDTO>> AssignDriverAsync(string clusterId, string driverId)
        {
            var entity = await _clusterRepo.GetByIdWithIncludesAsync(
                clusterId,
                c => c.AssignedDriver,
                c => c.AssignedDriver.User,
                c => c.TechCompany,
                c => c.TechCompany.User
            );

            if (entity == null)
                return new GeneralResponse<DeliveryClusterDTO> { Success = false, Message = "Cluster not found." };

            var previousDriverId = entity.AssignedDriverId;

            entity.AssignedDriverId = driverId;
            entity.AssignmentTime = DateTime.Now;
            _clusterRepo.Update(entity);
            await _clusterRepo.SaveChangesAsync();

            var dtoResult = DeliveryClusterMapper.ToDTO(entity);

            // Notify Admin
            await _notificationService.SendNotificationToRoleAsync(
                "Admin",
                NotificationType.DeliveryAssigned,
                entity.Id,
                "DeliveryCluster",
                $"Driver assigned to delivery cluster #{entity.Id}."
            );

            // Notify newly assigned driver
            if (!string.IsNullOrWhiteSpace(driverId) && driverId != previousDriverId)
            {
                await _notificationService.SendNotificationAsync(
                    driverId,
                    NotificationType.DeliveryAssigned,
                    entity.Id,
                    "DeliveryCluster",
                    $"You have been assigned to delivery cluster #{entity.Id}."
                );
            }

            // Optionally notify TechCompany
            if (entity.TechCompany?.UserId != null)
            {
                await _notificationService.SendNotificationAsync(
                    entity.TechCompany.UserId,
                    NotificationType.SystemAlert,
                    entity.Id,
                    "DeliveryCluster",
                    $"A driver has been assigned to delivery cluster #{entity.Id}."
                );
            }

            return new GeneralResponse<DeliveryClusterDTO>
            {
                Success = true,
                Message = "Driver assigned successfully.",
                Data = dtoResult
            };
        }

        public async Task AutoAssignDriverAsync(Delivery delivery, string clusterId, double? overrideLat = null, double? overrideLon = null)
        {
            if (delivery == null || string.IsNullOrWhiteSpace(clusterId))
                throw new ArgumentNullException("Delivery and clusterId are required.");

            var clusterResult = await GetByIdAsync(clusterId);
            if (!clusterResult.Success || clusterResult.Data == null)
                throw new InvalidOperationException("Cluster not found.");

            var clusterDto = clusterResult.Data;

            double locationLat, locationLon;

            if (overrideLat.HasValue && overrideLon.HasValue)
            {
                locationLat = overrideLat.Value;
                locationLon = overrideLon.Value;
            }
            else if (!string.IsNullOrWhiteSpace(clusterDto.TechCompanyId))
            {
                var techCompany = await _techCompanyRepo.GetByIdWithIncludesAsync(
                    clusterDto.TechCompanyId,
                    t => t.User
                );

                if (techCompany?.User == null || !techCompany.User.Latitude.HasValue || !techCompany.User.Longitude.HasValue)
                    throw new InvalidOperationException("Tech company coordinates missing.");

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

            var availableDriversResp = await _deliveryService.GetAvailableDeliveryPersonsAsync();
            var availableDrivers = availableDriversResp.Success ? availableDriversResp.Data?.ToList() ?? new List<DeliveryPersonReadDTO>() : new List<DeliveryPersonReadDTO>();
            if (!availableDrivers.Any()) throw new InvalidOperationException("No available drivers found.");

            var candidates = availableDrivers
                .Where(d => d.CurrentLatitude.HasValue && d.CurrentLongitude.HasValue)
                .Select(d => new { Driver = d, DistanceKm = _locationService.CalculateDistance(locationLat, locationLon, d.Latitude.Value, d.Longitude.Value) })
                .OrderBy(x => x.DistanceKm)
                .ToList();

            if (!candidates.Any()) throw new InvalidOperationException("No drivers with location data.");

            var best = candidates.First();
            if (_settings.MaxDriverDistanceKm > 0 && best.DistanceKm > _settings.MaxDriverDistanceKm)
                throw new InvalidOperationException($"Nearest driver {best.DistanceKm:F1} km exceeds max allowed distance.");

            await AssignDriverAsync(clusterId, best.Driver.Id);

            // Update delivery
            delivery.DeliveryPersonId = best.Driver.Id;
            delivery.Status = DeliveryStatus.Assigned;
            _deliveryRepo.Update(delivery);
            await _deliveryRepo.SaveChangesAsync();

            // Send notification to the assigned driver
            await _notificationService.SendNotificationAsync(
                best.Driver.Id,
                NotificationType.DeliveryAssigned,
                delivery.Id,
                "Delivery",
                $"New delivery assigned: #{delivery.TrackingNumber ?? delivery.Id}"
            );
        }

        public async Task<GeneralResponse<DeliveryClusterDTO>> SplitClusterAsync(
            Delivery delivery,
            DeliveryClusterDTO cluster,
            DeliveryPersonReadDTO driver)
        {
            if (delivery == null) throw new ArgumentNullException(nameof(delivery));
            if (cluster == null) throw new ArgumentNullException(nameof(cluster));
            if (driver == null) throw new ArgumentNullException(nameof(driver));

            if (string.IsNullOrWhiteSpace(cluster.TechCompanyId))
                return new GeneralResponse<DeliveryClusterDTO>
                {
                    Success = false,
                    Message = "Cluster has no TechCompany, cannot split."
                };

            var techCompany = await _techCompanyRepo.GetByIdAsync(cluster.TechCompanyId);
            if (techCompany == null || !techCompany.User.Latitude.HasValue || !techCompany.User.Longitude.HasValue)
                return new GeneralResponse<DeliveryClusterDTO>
                {
                    Success = false,
                    Message = "Tech company location missing."
                };

            var companyLat = techCompany.User.Latitude.Value;
            var companyLon = techCompany.User.Longitude.Value;

            var driverLat = driver.CurrentLatitude ?? throw new InvalidOperationException("Driver location missing.");
            var driverLon = driver.CurrentLongitude ?? throw new InvalidOperationException("Driver location missing.");

            var distanceToCompany = _locationService.CalculateDistance(driverLat, driverLon, companyLat, companyLon);

            if (distanceToCompany <= _settings.MaxDriverDistanceKm)
                return new GeneralResponse<DeliveryClusterDTO>
                {
                    Success = false,
                    Message = "No split required, driver is within allowed distance."
                };

            var customerLat = cluster.DropoffLatitude ?? delivery.DropoffLatitude ?? throw new InvalidOperationException("Customer location missing.");
            var customerLon = cluster.DropoffLongitude ?? delivery.DropoffLongitude ?? throw new InvalidOperationException("Customer location missing.");
            var (handoverLat, handoverLon) = _locationService.GetMidpoint(companyLat, companyLon, customerLat, customerLon);

            var pickupDto = new DeliveryClusterCreateDTO
            {
                DeliveryId = delivery.Id,
                TechCompanyId = cluster.TechCompanyId,
                TechCompanyName = cluster.TechCompanyName,
                DistanceKm = _locationService.CalculateDistance(companyLat, companyLon, handoverLat, handoverLon),
                Price = cluster.Price / 2,
                DropoffLatitude = handoverLat,
                DropoffLongitude = handoverLon,
                SequenceOrder = cluster.SequenceOrder
            };
            var pickupResult = await CreateClusterAsync(delivery.Id, pickupDto);
            if (!pickupResult.Success) return pickupResult;

            await _deliveryService.AutoAssignDriverAsync(delivery, pickupResult.Data.Id);

            var deliveryDto = new DeliveryClusterCreateDTO
            {
                DeliveryId = delivery.Id,
                DistanceKm = _locationService.CalculateDistance(handoverLat, handoverLon, customerLat, customerLon),
                Price = cluster.Price / 2,
                PickupLatitude = handoverLat,
                PickupLongitude = handoverLon,
                DropoffLatitude = customerLat,
                DropoffLongitude = customerLon,
                SequenceOrder = cluster.SequenceOrder + 1,
                AssignedDriverId = driver.Id
            };
            var deliveryLegResult = await CreateClusterAsync(delivery.Id, deliveryDto);
            if (!deliveryLegResult.Success) return deliveryLegResult;

            await DeleteClusterAsync(cluster.Id);

            return deliveryLegResult;
        }

        public async Task<GeneralResponse<DeliveryClusterDTO>> GetTrackingAsync(string clusterId)
        {
            var cluster = await _clusterRepo.GetByIdWithIncludesAsync(clusterId, c => c.Tracking);

            var dto = DeliveryClusterMapper.ToDTO(cluster);
            dto.Tracking = cluster.Tracking == null ? null : new DeliveryClusterTrackingDTO
            {
                ClusterId = cluster.Id,
                DeliveryId = cluster.DeliveryId,
                TechCompanyId = cluster.TechCompanyId,
                TechCompanyName = cluster.TechCompanyName ?? cluster.TechCompany?.User?.FullName,
                DistanceKm = cluster.DistanceKm,
                Price = cluster.Price,
                AssignedDriverId = cluster.AssignedDriverId,
                DriverName = cluster.AssignedDriverName ?? cluster.AssignedDriver?.User?.FullName,
                AssignmentTime = cluster.AssignmentTime,
                DropoffLatitude = cluster.DropoffLatitude,
                DropoffLongitude = cluster.DropoffLongitude,
                SequenceOrder = cluster.SequenceOrder,
                EstimatedDistance = cluster.EstimatedDistance,
                EstimatedPrice = cluster.EstimatedPrice,
                Status = cluster.Tracking.Status,
                Location = cluster.Tracking?.Location,
                LastUpdated = cluster.Tracking?.LastUpdated ?? DateTime.Now,
                PickupConfirmed = cluster.PickupConfirmed,
                PickupConfirmedAt = cluster.PickupConfirmedAt
            };

            return new GeneralResponse<DeliveryClusterDTO> { Success = true, Data = dto };
        }

        public async Task<GeneralResponse<DeliveryClusterDTO>> UpdateClusterTrackingAsync(
           string clusterId,
           DeliveryClusterTrackingDTO trackingDto)
        {
            var cluster = await _clusterRepo.GetByIdWithIncludesAsync(
                clusterId,
                c => c.AssignedDriver,
                c => c.AssignedDriver.User
            );

            if (cluster == null)
            {
                return new GeneralResponse<DeliveryClusterDTO>
                {
                    Success = false,
                    Message = "Cluster not found.",
                    Data = null
                };
            }

            if (cluster.Tracking == null)
            {
                cluster.Tracking = new DeliveryClusterTracking
                {
                    clusterId = cluster.Id
                };
            }

            var previousStatus = cluster.Tracking.Status;

            cluster.Tracking.Location = trackingDto.Location ?? cluster.Tracking.Location;
            cluster.Tracking.LastUpdated = trackingDto.LastUpdated == default ? DateTime.UtcNow : trackingDto.LastUpdated;
            cluster.Tracking.Status = trackingDto.Status;
            cluster.Tracking.Driver = trackingDto.DriverName ?? cluster.Tracking.Driver;

            cluster.UpdatedAt = DateTime.Now;

            _clusterRepo.Update(cluster);
            await _clusterRepo.SaveChangesAsync();

            // Push notification to assigned driver if status changed
            if (!string.IsNullOrWhiteSpace(cluster.AssignedDriverId) && previousStatus != trackingDto.Status)
            {
                await _notificationService.SendNotificationAsync(
                    cluster.AssignedDriverId,
                    NotificationType.DeliveryTrackingUpdated,
                    cluster.DeliveryId,
                    "Delivery",
                    $"Cluster #{cluster.Id} status updated: {trackingDto.Status}"
                );
            }

            var dto = DeliveryClusterMapper.ToDTO(cluster);
            return new GeneralResponse<DeliveryClusterDTO>
            {
                Success = true,
                Message = "Tracking updated successfully.",
                Data = dto
            };
        }

        public async Task<GeneralResponse<IEnumerable<DeliveryClusterDTO>>> GetUnassignedClustersAsync()
        {
            var clusters = await _clusterRepo.FindWithIncludesAsync(
                c => string.IsNullOrEmpty(c.AssignedDriverId),
                c => c.TechCompany,
                c => c.TechCompany.User,
                c => c.Tracking,
                c => c.DriverOffers,
                c => c.DriverOffers.Select(o => o.Driver)
            );

            var clusterDtos = clusters.Select(DeliveryClusterMapper.ToDTO);
            return new GeneralResponse<IEnumerable<DeliveryClusterDTO>>
            {
                Success = true,
                Data = clusterDtos
            };
        }
    }
}
