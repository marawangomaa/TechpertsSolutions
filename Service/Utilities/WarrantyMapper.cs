using Core.DTOs.WarrantyDTOs;
using TechpertsSolutions.Core.Entities;

namespace Service.Utilities
{
    public static class WarrantyMapper
    {
        public static WarrantyReadDTO MapToWarrantyReadDTO(Warranty entity)
        {
            if (entity == null)
                return null!;

            return new WarrantyReadDTO
            {
                Id = entity.Id,
                Type = entity.Type,
                Duration = entity.Duration,
                Description = entity.Description,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                ProductId = entity.ProductId,
            };
        }

        public static Warranty MapToEntity(WarrantyCreateDTO dto)
        {
            if (dto == null)
                return null!;

            return new Warranty
            {
                Id = Guid.NewGuid().ToString(),
                Type = dto.Type,
                Duration = dto.Duration,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                ProductId = dto.ProductId,
            };
        }

        public static void UpdateEntity(Warranty entity, WarrantyUpdateDTO dto)
        {
            if (entity == null || dto == null)
                return;

            if (!string.IsNullOrWhiteSpace(dto.Type))
                entity.Type = dto.Type;

            if (!string.IsNullOrWhiteSpace(dto.Duration))
                entity.Duration = dto.Duration;

            if (!string.IsNullOrWhiteSpace(dto.Description))
                entity.Description = dto.Description;

            if (dto.StartDate.HasValue)
                entity.StartDate = dto.StartDate.Value;

            if (dto.EndDate.HasValue)
                entity.EndDate = dto.EndDate.Value;

            if (!string.IsNullOrWhiteSpace(dto.ProductId))
                entity.ProductId = dto.ProductId;
        }
    }
}
