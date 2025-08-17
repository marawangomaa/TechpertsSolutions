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
                Website = entity.Website,
                Description = entity.Description,
                Longitude = entity.User?.Longitude,
                Latitude = entity.User?.Latitude,
                PostalCode = entity.User?.PostalCode,
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
                UserId = dto.UserId,
                RoleId = dto.RoleId
            };
        }

        public static void UpdateEntity(TechCompany entity, TechCompanyUpdateDTO dto)
        {
            if (entity == null || dto == null) return;

            if (!string.IsNullOrWhiteSpace(dto.Website))
                entity.Website = dto.Website;
            if (!string.IsNullOrWhiteSpace(dto.Description))
                entity.Description = dto.Description;
        }
    }
}