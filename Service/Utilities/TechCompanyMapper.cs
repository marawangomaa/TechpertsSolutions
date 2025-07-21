using Core.DTOs.TechCompany;
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
                City = entity.City,
                Country = entity.Country,
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
                City = dto.City,
                Country = dto.Country,
                UserId = dto.UserId,
                RoleId = dto.RoleId
            };
        }

        public static void UpdateEntity(TechCompany entity, TechCompanyUpdateDTO dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.MapLocation))
                entity.MapLocation = dto.MapLocation;

            if (!string.IsNullOrWhiteSpace(dto.City))
                entity.City = dto.City;

            if (!string.IsNullOrWhiteSpace(dto.Country))
                entity.Country = dto.Country;

            if (!string.IsNullOrWhiteSpace(dto.UserId))
                entity.UserId = dto.UserId;

            if (!string.IsNullOrWhiteSpace(dto.RoleId))
                entity.RoleId = dto.RoleId;
        }
    }
}
