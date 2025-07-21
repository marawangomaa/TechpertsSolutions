using Core.DTOs.SalesManager;
using Core.Interfaces;
using Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.DTOs;
using TechpertsSolutions.Core.Entities;

namespace Service
{
    public class SalesManagerService : ISalesManagerService
    {
        private readonly IRepository<SalesManager> _salesManagerRepo;

        public SalesManagerService(IRepository<SalesManager> salesManagerRepo)
        {
            _salesManagerRepo = salesManagerRepo;
        }

        public async Task<GeneralResponse<SalesManagerReadDTO>> CreateAsync(SalesManagerCreateDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.UserId) || string.IsNullOrWhiteSpace(dto.RoleId))
            {
                return new GeneralResponse<SalesManagerReadDTO> { Success = false, Message = "Invalid data.", Data = null };
            }

            var entity = new SalesManager
            {
                Id = Guid.NewGuid().ToString(),
                UserId = dto.UserId,
                RoleId = dto.RoleId
            };

            await _salesManagerRepo.AddAsync(entity);
            await _salesManagerRepo.SaveChangesAsync();

            return new GeneralResponse<SalesManagerReadDTO>
            {
                Success = true,
                Message = "Sales Manager created successfully.",
                Data = new SalesManagerReadDTO { Id = entity.Id, UserId = entity.UserId, RoleId = entity.RoleId }
            };
        }

        public async Task<GeneralResponse<SalesManagerReadDTO>> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return new GeneralResponse<SalesManagerReadDTO> { Success = false, Message = "ID cannot be empty.", Data = null };

            var entity = await _salesManagerRepo.GetByIdWithIncludesAsync(id, sm => sm.User, sm => sm.Role);

            if (entity == null)
                return new GeneralResponse<SalesManagerReadDTO> { Success = false, Message = "Sales Manager not found.", Data = null };

            return new GeneralResponse<SalesManagerReadDTO>
            {
                Success = true,
                Message = "Sales Manager retrieved successfully.",
                Data = new SalesManagerReadDTO { Id = entity.Id, UserId = entity.UserId, RoleId = entity.RoleId }
            };
        }

        public async Task<GeneralResponse<IEnumerable<SalesManagerReadDTO>>> GetAllAsync()
        {
            var entities = await _salesManagerRepo.GetAllWithIncludesAsync(sm => sm.User, sm => sm.Role);

            var dtos = entities.Select(e => new SalesManagerReadDTO { Id = e.Id, UserId = e.UserId, RoleId = e.RoleId });

            return new GeneralResponse<IEnumerable<SalesManagerReadDTO>>
            {
                Success = true,
                Message = "Sales Managers retrieved successfully.",
                Data = dtos
            };
        }

        public async Task<GeneralResponse<SalesManagerReadDTO>> UpdateAsync(string id, SalesManagerUpdateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(id) || dto == null)
                return new GeneralResponse<SalesManagerReadDTO> { Success = false, Message = "Invalid input.", Data = null };

            var entity = await _salesManagerRepo.GetByIdAsync(id);

            if (entity == null)
                return new GeneralResponse<SalesManagerReadDTO> { Success = false, Message = "Sales Manager not found.", Data = null };

            if (!string.IsNullOrWhiteSpace(dto.UserId))
                entity.UserId = dto.UserId;

            if (!string.IsNullOrWhiteSpace(dto.RoleId))
                entity.RoleId = dto.RoleId;

            _salesManagerRepo.Update(entity);
            await _salesManagerRepo.SaveChangesAsync();

            return new GeneralResponse<SalesManagerReadDTO>
            {
                Success = true,
                Message = "Sales Manager updated successfully.",
                Data = new SalesManagerReadDTO { Id = entity.Id, UserId = entity.UserId, RoleId = entity.RoleId }
            };
        }

        public async Task<GeneralResponse<string>> DeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return new GeneralResponse<string> { Success = false, Message = "ID cannot be empty.", Data = null };

            var entity = await _salesManagerRepo.GetByIdAsync(id);

            if (entity == null)
                return new GeneralResponse<string> { Success = false, Message = "Sales Manager not found.", Data = null };

            _salesManagerRepo.Remove(entity);
            await _salesManagerRepo.SaveChangesAsync();

            return new GeneralResponse<string>
            {
                Success = true,
                Message = "Sales Manager deleted successfully.",
                Data = id
            };
        }
    }
}
