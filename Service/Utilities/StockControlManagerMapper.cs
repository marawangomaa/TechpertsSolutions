using Core.DTOs.StockControlManagerDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Service.Utilities
{
    public static class StockControlManagerMapper
    {
        public static StockControlManagerReadDTO ToReadDTO(StockControlManager entity)
        {
            return new StockControlManagerReadDTO
            {
                Id = entity.Id,
                UserId = entity.UserId,
                RoleId = entity.RoleId,
                UserName = entity.User?.UserName,
                RoleName = entity.Role?.Name
            };
        }

        public static StockControlManager ToEntity(StockControlManagerCreateDTO dto)
        {
            return new StockControlManager
            {
                Id = Guid.NewGuid().ToString(),
                UserId = dto.UserId,
                RoleId = dto.RoleId
            };
        }

        public static void UpdateEntity(StockControlManager entity, StockControlManagerUpdateDTO dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.UserId))
                entity.UserId = dto.UserId;

            if (!string.IsNullOrWhiteSpace(dto.RoleId))
                entity.RoleId = dto.RoleId;
        }
    }
}
