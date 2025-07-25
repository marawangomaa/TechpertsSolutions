using Core.DTOs.TechManagerDTOs;
using Core.Interfaces;
using Core.Interfaces.Services;
using Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.DTOs;
using TechpertsSolutions.Core.Entities;

namespace Service
{
    public class TechManagerService : ITechManagerService
    {
        private readonly IRepository<TechManager> _techMangerRepo;

        public TechManagerService(IRepository<TechManager> repo)
        {
            _techMangerRepo = repo;
        }

        public async Task<GeneralResponse<TechManagerReadDTO>> CreateAsync(TechManagerCreateDTO dto)
        {
            var entity = TechManagerMapper.ToEntity(dto);
            await _techMangerRepo.AddAsync(entity);
            await _techMangerRepo.SaveChangesAsync();

            return new GeneralResponse<TechManagerReadDTO>
            {
                Success = true,
                Message = "TechManager created successfully.",
                Data = TechManagerMapper.ToReadDTO(entity)
            };
        }

        public async Task<GeneralResponse<TechManagerReadDTO>> GetByIdAsync(string id)
        {
            var entity = await _techMangerRepo.GetByIdWithIncludesAsync(id, t => t.User, t => t.Role);
            if (entity == null)
            {
                return new GeneralResponse<TechManagerReadDTO>
                {
                    Success = false,
                    Message = "TechManager not found.",
                    Data = null
                };
            }

            return new GeneralResponse<TechManagerReadDTO>
            {
                Success = true,
                Message = "Retrieved successfully.",
                Data = TechManagerMapper.ToReadDTO(entity)
            };
        }

        public async Task<GeneralResponse<IEnumerable<TechManagerReadDTO>>> GetAllAsync()
        {
            var entities = await _techMangerRepo.GetAllWithIncludesAsync(t => t.User, t => t.Role);
            return new GeneralResponse<IEnumerable<TechManagerReadDTO>>
            {
                Success = true,
                Message = "All TechManagers retrieved successfully.",
                Data = entities.Select(TechManagerMapper.ToReadDTO)
            };
        }

        public async Task<GeneralResponse<TechManagerReadDTO>> UpdateAsync(string id, TechManagerUpdateDTO dto)
        {
            var entity = await _techMangerRepo.GetByIdAsync(id);
            if (entity == null)
            {
                return new GeneralResponse<TechManagerReadDTO>
                {
                    Success = false,
                    Message = "TechManager not found.",
                    Data = null
                };
            }

            TechManagerMapper.UpdateEntity(entity, dto);
            _techMangerRepo.Update(entity);
            await _techMangerRepo.SaveChangesAsync();

            return new GeneralResponse<TechManagerReadDTO>
            {
                Success = true,
                Message = "Updated successfully.",
                Data = TechManagerMapper.ToReadDTO(entity)
            };
        }

        public async Task<GeneralResponse<string>> DeleteAsync(string id)
        {
            var entity = await _techMangerRepo.GetByIdAsync(id);
            if (entity == null)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "TechManager not found.",
                    Data = null
                };
            }

            _techMangerRepo.Remove(entity);
            await _techMangerRepo.SaveChangesAsync();

            return new GeneralResponse<string>
            {
                Success = true,
                Message = "Deleted successfully.",
                Data = id
            };
        }
    }

}
