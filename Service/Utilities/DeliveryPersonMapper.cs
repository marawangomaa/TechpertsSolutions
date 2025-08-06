using Core.DTOs.DeliveryPersonDTOs;
using TechpertsSolutions.Core.Entities;
using System;

namespace Service.Utilities
{
    public static class DeliveryPersonMapper
    {
        public static DeliveryPersonReadDTO ToReadDTO(DeliveryPerson entity)
        {
            if (entity == null) return null;

            var user = entity.User;
            var role = entity.Role;

            return new DeliveryPersonReadDTO
            {
                Id = entity.Id,
                UserId = entity.UserId,
                RoleId = entity.RoleId,
                VehicleNumber = entity.VehicleNumber,
                VehicleType = entity.VehicleType,
                IsAvailable = entity.IsAvailable,

                // User properties with null checks
                PhoneNumber = user?.PhoneNumber ?? "Unknown",
                City = user?.City ?? "Unknown",
                Country = user?.Country ?? "Unknown",
                UserName = user?.UserName ?? "Unknown",
                UserFullName = user?.FullName ?? "Unknown",

                // Role name with null check
                RoleName = role?.Name ?? "Unknown"
            };
        }

        public static DeliveryPersonReadDTO MapToDeliveryPersonReadDTO(DeliveryPerson entity)
        {
            return ToReadDTO(entity); // Avoid duplicate logic
        }

        public static DeliveryPerson ToEntity(DeliveryPersonCreateDTO dto)
        {
            if (dto == null) return null;

            return new DeliveryPerson
            {
                Id = Guid.NewGuid().ToString(),
                UserId = dto.UserId,
                RoleId = dto.RoleId,
                VehicleNumber = dto.VehicleNumber,
                VehicleType = dto.VehicleType,
                IsAvailable = true
            };
        }

        public static void UpdateEntity(DeliveryPerson entity, DeliveryPersonUpdateDTO dto)
        {
            if (entity == null || dto == null) return;

            if (!string.IsNullOrWhiteSpace(dto.VehicleNumber))
                entity.VehicleNumber = dto.VehicleNumber;

            if (!string.IsNullOrWhiteSpace(dto.VehicleType))
                entity.VehicleType = dto.VehicleType;

            if (dto.IsAvailable.HasValue)
                entity.IsAvailable = dto.IsAvailable.Value;
        }
    }
}