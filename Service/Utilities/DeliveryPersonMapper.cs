using Core.DTOs.DeliveryPersonDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Service.Utilities
{
    public static class DeliveryPersonMapper
    {
        public static DeliveryPersonReadDTO ToReadDTO(DeliveryPerson entity)
        {
            if (entity == null) return null;

            return new DeliveryPersonReadDTO
            {
                Id = entity.Id,
                UserId = entity.UserId,
                RoleId = entity.RoleId,
                VehicleNumber = entity.VehicleNumber,
                VehicleType = entity.VehicleType,
                PhoneNumber = entity.PhoneNumber,
                City = entity.City,
                Country = entity.Country,
                IsAvailable = entity.IsAvailable,
                UserName = entity.User?.UserName,
                UserFullName = entity.User?.FullName,
                RoleName = entity.Role?.Name
            };
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
                PhoneNumber = dto.PhoneNumber,
                City = dto.City,
                Country = dto.Country,
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

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                entity.PhoneNumber = dto.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(dto.City))
                entity.City = dto.City;

            if (!string.IsNullOrWhiteSpace(dto.Country))
                entity.Country = dto.Country;

            if (dto.IsAvailable.HasValue)
                entity.IsAvailable = dto.IsAvailable.Value;
        }
    }
} 