using Core.DTOs.PCAssemblyDTOs;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Service.Utilities
{
    public static class PCAssemblyMapper
    {
        public static PCAssemblyReadDTO ToReadDTO(PCAssembly entity)
        {
            if (entity == null) return null!;

            return new PCAssemblyReadDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                CreatedAt = entity.CreatedAt,
                CustomerId = entity.CustomerId,
                ServiceUsageId = entity.ServiceUsageId,
                Items = entity.PCAssemblyItems?.Select(item => new PCAssemblyItemReadDTO
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    Total = item.Total
                }).ToList() ?? new List<PCAssemblyItemReadDTO>()
            };
        }

        public static PCAssembly ToEntity(PCAssemblyCreateDTO dto)
        {
            if (dto == null) return null!;

            return new PCAssembly
            {
                Id = Guid.NewGuid().ToString(),
                Name = dto.Name,
                CustomerId = dto.CustomerId,
                ServiceUsageId = dto.ServiceUsageId,
                CreatedAt = DateTime.UtcNow,
                PCAssemblyItems = dto.Items?.Select(item => new PCAssemblyItem
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    Total = item.Quantity * item.Price
                }).ToList() ?? new List<PCAssemblyItem>()
            };
        }

        public static void UpdateEntity(PCAssembly entity, PCAssemblyUpdateDTO dto)
        {
            if (entity == null || dto == null) return;

            if (!string.IsNullOrWhiteSpace(dto.Name))
                entity.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.ServiceUsageId))
                entity.ServiceUsageId = dto.ServiceUsageId;
        }
    }
}
