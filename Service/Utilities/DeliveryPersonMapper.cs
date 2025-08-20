using Core.DTOs.DeliveryDTOs;
using Core.DTOs.DeliveryPersonDTOs;
using Core.Entities;
using Core.Enums;
using TechpertsSolutions.Core.Entities;

namespace Service.Utilities
{
    public static class DeliveryPersonMapper
    {
        public static DeliveryPersonReadDTO ToReadDTO(DeliveryPerson entity)
        {
            if (entity == null)
                return null;

            var user = entity.User;
            var role = entity.Role;

            return new DeliveryPersonReadDTO
            {
                Id = entity.Id,
                UserId = entity.UserId,
                RoleId = entity.RoleId,
                VehicleNumber = entity.VehicleNumber,
                VehicleType = entity.VehicleType,
                VehicleImage = entity.VehicleImage,
                AccountStatus = entity.AccountStatus,
                IsAvailable = entity.IsAvailable,
                PostalCode = user?.PostalCode ?? "Unknown",
                Longitude = user?.Longitude,
                Latitude = user?.Latitude,
                CurrentLatitude = entity.CurrentLatitude,
                CurrentLongitude = entity.CurrentLongitude,
                PhoneNumber = user?.PhoneNumber ?? "Unknown",
                City = user?.City ?? "Unknown",
                Country = user?.Country ?? "Unknown",
                UserName = user?.UserName ?? "Unknown",
                UserFullName = user?.FullName ?? "Unknown",
                UserImage = user?.ProfilePhotoUrl ?? "Unknown",
                RoleName = role?.Name ?? "Unknown",
            };
        }

        public static DeliveryPersonReadDTO MapToDeliveryPersonReadDTO(DeliveryPerson entity)
        {
            return ToReadDTO(entity);
        }

        public static DeliveryOfferDTO ToOfferDTO(DeliveryOffer offer)
        {
            if (offer == null) return null;

            return new DeliveryOfferDTO
            {
                Id = offer.Id,
                DeliveryId = offer.DeliveryId,
                ClusterId = offer.ClusterId,
                DeliveryPersonId = offer.DeliveryPersonId,
                Status = offer.Status,
                CreatedAt = offer.CreatedAt,
                ExpiryTime = offer.ExpiryTime,
                IsActive = offer.IsActive,

                TechCompanies = offer.Delivery?.TechCompanies?.ToList() ?? new List<TechCompany>(),

                DeliveryTrackingNumber = offer.Delivery?.TrackingNumber,
                CustomerName = offer.Delivery?.CustomerName,
                DeliveryLatitude = offer.Delivery?.DropoffLatitude,
                DeliveryLongitude = offer.Delivery?.DropoffLongitude
            };
        }

        public static DeliveryPerson ToEntity(DeliveryPersonCreateDTO dto)
        {
            if (dto == null)
                return null;

            return new DeliveryPerson
            {
                Id = Guid.NewGuid().ToString(),
                UserId = dto.UserId,
                RoleId = dto.RoleId,
                VehicleNumber = dto.VehicleNumber,
                VehicleType = dto.VehicleType,
                IsAvailable = true,
            };
        }

        public static void UpdateEntity(DeliveryPerson entity, DeliveryPersonStatus AccountStatus, DeliveryPersonUpdateDTO dto)
        {
            if (entity == null || dto == null)
                return;

            if (!string.IsNullOrWhiteSpace(dto.VehicleNumber))
                entity.VehicleNumber = dto.VehicleNumber;

            if (!string.IsNullOrWhiteSpace(dto.VehicleType))
                entity.VehicleType = dto.VehicleType;

            if (!string.IsNullOrWhiteSpace(dto.VehicleImage))
                entity.VehicleImage = dto.VehicleImage;

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                entity.User.PhoneNumber = dto.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(dto.City))
                entity.User.City = dto.City;

            if (!string.IsNullOrWhiteSpace(dto.Country))
                entity.User.Country = dto.Country;

            if (dto.IsAvailable.HasValue)
                entity.IsAvailable = dto.IsAvailable.Value;

            entity.AccountStatus = AccountStatus;
        }

        public static DeliveryOfferDTO ToDTO(DeliveryOffer offer)
        {
            if (offer == null)
                return null;

            return new DeliveryOfferDTO
            {
                Id = offer.Id,
                DeliveryId = offer.DeliveryId,
                ClusterId = offer.ClusterId,
                DeliveryPersonId = offer.DeliveryPersonId,
                Status = offer.Status,
                CreatedAt = offer.CreatedAt,
                ExpiryTime = offer.ExpiryTime,
                IsActive = offer.IsActive,
                DeliveryTrackingNumber = offer.Delivery?.TrackingNumber,
                DeliveryLatitude = offer.Delivery?.DropoffLatitude,
                DeliveryLongitude = offer.Delivery?.DropoffLongitude,
                CustomerName = offer.Delivery?.CustomerName
            };
        }

        public static DeliveryOffer ToEntity(DeliveryOfferDTO dto)
        {
            if (dto == null)
                return null;

            return new DeliveryOffer
            {
                Id = dto.Id,
                DeliveryId = dto.DeliveryId,
                ClusterId = dto.ClusterId,
                DeliveryPersonId = dto.DeliveryPersonId,
                Status = dto.Status,
                CreatedAt = dto.CreatedAt,
                ExpiryTime = dto.ExpiryTime,
                IsActive = dto.IsActive,
            };
        }
    }
}
