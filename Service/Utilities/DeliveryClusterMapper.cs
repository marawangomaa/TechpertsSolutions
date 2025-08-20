using Core.DTOs.DeliveryDTOs;
using Core.Entities;
using Core.Enums;

namespace Service.Utilities
{
    public static class DeliveryClusterMapper
    {
        public static DeliveryCluster ToEntity(DeliveryClusterCreateDTO dto)
        {
            if (dto == null)
                return null;

            return new DeliveryCluster
            {
                DeliveryId = dto.DeliveryId,
                TechCompanyId = dto.TechCompanyId,
                TechCompanyName = dto.TechCompanyName,
                DistanceKm = dto.DistanceKm,
                Price = dto.Price,
                DropoffLatitude = dto.DropoffLatitude,
                DropoffLongitude = dto.DropoffLongitude,
                PickupConfirmed = dto.PickupConfirmed,
                PickupConfirmedAt = dto.PickupConfirmedAt,
                SequenceOrder = dto.SequenceOrder,
                Status = DeliveryClusterStatus.Pending,
                EstimatedDistance = dto.DistanceKm,
                EstimatedPrice = dto.Price,
                Tracking =
                    dto.Tracking == null
                        ? null
                        : new DeliveryClusterTracking
                        {
                            Location = dto.Tracking.Location,
                            LastUpdated = dto.Tracking.LastUpdated,
                            Status =
                                dto.Tracking.Status != null
                                    ? Enum.Parse<DeliveryClusterStatus>(
                                        dto.Tracking.Status.ToString()
                                    )
                                    : DeliveryClusterStatus.Pending,
                            Driver = dto.Tracking.Driver,
                        },
            };
        }

        public static DeliveryClusterDTO ToDTO(DeliveryCluster entity)
        {
            if (entity == null)
                return null;

            return new DeliveryClusterDTO
            {
                Id = entity.Id,
                DeliveryId = entity.DeliveryId,
                TechCompanyId = entity.TechCompanyId,
                TechCompanyName = entity.TechCompanyName ?? entity.TechCompany?.User?.FullName,
                DistanceKm = entity.DistanceKm,
                Price = entity.Price,
                Status = entity.Status,
                AssignedDriverId = entity.AssignedDriverId,
                AssignedDriverName =
                    entity.AssignedDriverName ?? entity.AssignedDriver?.User?.FullName,
                AssignmentTime = entity.AssignmentTime,
                DropoffLatitude = entity.DropoffLatitude,
                DropoffLongitude = entity.DropoffLongitude,
                PickupLatitude = entity.Delivery?.PickupLatitude,
                PickupLongitude = entity.Delivery?.PickupLongitude,
                SequenceOrder = entity.SequenceOrder,
                DriverOfferCount = entity.DriverOffers?.Count ?? 0,
                EstimatedDistance = entity.EstimatedDistance,
                EstimatedPrice = entity.EstimatedPrice,
                Tracking =
                    entity.Tracking == null
                        ? null
                        : new DeliveryClusterTrackingDTO
                        {
                            ClusterId = entity.Id,
                            DeliveryId = entity.DeliveryId,
                            TechCompanyId = entity.TechCompanyId,
                            TechCompanyName =
                                entity.TechCompanyName ?? entity.TechCompany?.User?.FullName,
                            DistanceKm = entity.DistanceKm,
                            Price = entity.Price,
                            AssignedDriverId = entity.AssignedDriverId,
                            DriverName =
                                entity.AssignedDriverName ?? entity.AssignedDriver?.User?.FullName,
                            AssignmentTime = entity.AssignmentTime,
                            DropoffLatitude = entity.DropoffLatitude,
                            DropoffLongitude = entity.DropoffLongitude,
                            SequenceOrder = entity.SequenceOrder,
                            EstimatedDistance = entity.EstimatedDistance,
                            EstimatedPrice = entity.EstimatedPrice,
                            Status = entity.Tracking.Status,
                            Location = entity.Tracking.Location,
                            LastUpdated = entity.Tracking.LastUpdated,
                            PickupConfirmed = entity.PickupConfirmed,
                            PickupConfirmedAt = entity.PickupConfirmedAt,
                        },
            };
        }

        public static void UpdateEntity(DeliveryCluster entity, DeliveryClusterDTO dto)
        {
            if (entity == null || dto == null)
                return;

            entity.TechCompanyId = dto.TechCompanyId ?? entity.TechCompanyId;
            entity.TechCompanyName = string.IsNullOrWhiteSpace(dto.TechCompanyName)
                ? entity.TechCompanyName
                : dto.TechCompanyName;

            entity.DistanceKm = dto.DistanceKm != default ? dto.DistanceKm : entity.DistanceKm;
            entity.Price = dto.Price != default ? dto.Price : entity.Price;

            entity.Status = dto.Status;

            entity.AssignedDriverId = dto.AssignedDriverId ?? entity.AssignedDriverId;
            entity.AssignedDriverName = string.IsNullOrWhiteSpace(dto.AssignedDriverName)
                ? entity.AssignedDriverName
                : dto.AssignedDriverName;
            entity.AssignmentTime = dto.AssignmentTime ?? entity.AssignmentTime;

            entity.DropoffLatitude = dto.DropoffLatitude ?? entity.DropoffLatitude;
            entity.DropoffLongitude = dto.DropoffLongitude ?? entity.DropoffLongitude;

            if (dto.PickupLatitude.HasValue)
                entity.Delivery.PickupLatitude = dto.PickupLatitude;
            if (dto.PickupLongitude.HasValue)
                entity.Delivery.PickupLongitude = dto.PickupLongitude;

            if (dto.SequenceOrder != default)
                entity.SequenceOrder = dto.SequenceOrder;
            if (dto.EstimatedDistance.HasValue)
                entity.EstimatedDistance = dto.EstimatedDistance.Value;
            if (dto.EstimatedPrice.HasValue)
                entity.EstimatedPrice = dto.EstimatedPrice.Value;

            entity.UpdatedAt = DateTime.Now;

            if (dto.Tracking != null)
            {
                entity.Tracking ??= new DeliveryClusterTracking();
                entity.Tracking.Location = dto.Tracking.Location;
                entity.Tracking.LastUpdated = dto.Tracking.LastUpdated;
                entity.Tracking.Status = dto.Tracking.Status;
                entity.Tracking.Driver = dto.Tracking.DriverName;
            }
        }
    }
}
