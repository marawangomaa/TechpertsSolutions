using Core.DTOs.TechCompanyDTOs;
using TechpertsSolutions.Core.Entities;

namespace Service.Utilities
{
    public static class TechCompanyMapper
    {
        public static TechCompanyReadDTO ToReadDTO(TechCompany entity)
        {
            if (entity == null) return null;

            return new TechCompanyReadDTO
            {
                Id = entity.Id,
                MapLocation = entity.MapLocation,
                City = entity.User?.City ?? "Unknown",
                Country = entity.User?.Country ?? "Unknown",
                UserId = entity.UserId,
                RoleId = entity.RoleId,
                UserName = entity.User?.UserName ?? "Unknown",
                RoleName = entity.Role?.Name ?? "Unknown"
            };
        }

        public static TechCompanyReadDTO MapToTechCompanyReadDTO(TechCompany entity)
        {
            // Reuse ToReadDTO to avoid duplication
            return ToReadDTO(entity);
        }

        public static TechCompany ToEntity(TechCompanyCreateDTO dto)
        {
            if (dto == null) return null;

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
            if (entity == null || dto == null) return;

            if (!string.IsNullOrWhiteSpace(dto.MapLocation))
                entity.MapLocation = dto.MapLocation;

            if (!string.IsNullOrWhiteSpace(dto.UserId))
                entity.UserId = dto.UserId;

            if (!string.IsNullOrWhiteSpace(dto.RoleId))
                entity.RoleId = dto.RoleId;
        }
    }
}