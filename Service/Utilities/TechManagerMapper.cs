using Core.DTOs.TechManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Service.Utilities
{
    public static class TechManagerMapper
    {
        public static TechManagerReadDTO ToReadDTO(TechManager entity)
        {
            return new TechManagerReadDTO
            {
                Id = entity.Id,
                Specialization = entity.Specialization,
                UserId = entity.UserId,
                RoleId = entity.RoleId,
                UserName = entity.User?.UserName,
                RoleName = entity.Role?.Name
            };
        }

        public static TechManager ToEntity(TechManagerCreateDTO dto)
        {
            return new TechManager
            {
                Id = Guid.NewGuid().ToString(),
                Specialization = dto.Specialization,
                UserId = dto.UserId,
                RoleId = dto.RoleId
            };
        }

        public static void UpdateEntity(TechManager entity, TechManagerUpdateDTO dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.Specialization))
                entity.Specialization = dto.Specialization;

            if (!string.IsNullOrWhiteSpace(dto.UserId))
                entity.UserId = dto.UserId;

            if (!string.IsNullOrWhiteSpace(dto.RoleId))
                entity.RoleId = dto.RoleId;
        }
    }
}
