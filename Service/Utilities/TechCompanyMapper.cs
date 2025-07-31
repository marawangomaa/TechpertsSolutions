using Core.DTOs.TechCompanyDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Service.Utilities
{
    public static class TechCompanyMapper
    {
        public static TechCompanyReadDTO ToReadDTO(TechCompany entity)
        {
            return new TechCompanyReadDTO
            {
                Id = entity.Id,
                MapLocation = entity.MapLocation,
                City = entity.User.City,
                Country = entity.User.Country,
                UserId = entity.UserId,
                RoleId = entity.RoleId,
                UserName = entity.User?.UserName,
                RoleName = entity.Role?.Name
            };
        }

        public static TechCompanyReadDTO MapToTechCompanyReadDTO(TechCompany entity)
        {
            return new TechCompanyReadDTO
            {
                Id = entity.Id,
                MapLocation = entity.MapLocation,
                City = entity.User?.City,
                Country = entity.User?.Country,
                UserId = entity.UserId,
                RoleId = entity.RoleId,
                UserName = entity.User?.UserName,
                RoleName = entity.Role?.Name
            };
        }

        public static TechCompany ToEntity(TechCompanyCreateDTO dto)
        {
            return new TechCompany
            {
                Id = Guid.NewGuid().ToString(),
                MapLocation = dto.MapLocation,
                UserId = dto.UserId,
                RoleId = dto.RoleId
            };
        }

        public static void UpdateEntity(TechCompany entity, TechCompanyUpdateDTO dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.MapLocation))
                entity.MapLocation = dto.MapLocation;

            if (!string.IsNullOrWhiteSpace(dto.UserId))
                entity.UserId = dto.UserId;

            if (!string.IsNullOrWhiteSpace(dto.RoleId))
                entity.RoleId = dto.RoleId;
        }
    }
}
