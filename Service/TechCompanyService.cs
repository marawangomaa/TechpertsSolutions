using Core.DTOs.TechCompanyDTOs;
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
    public class TechCompanyService : ITechCompanyService
    {
        private readonly IRepository<TechCompany> _techCompanyRepo;

        public TechCompanyService(IRepository<TechCompany> repo)
        {
            _techCompanyRepo = repo;
        }

        public async Task<GeneralResponse<TechCompanyReadDTO>> CreateAsync(TechCompanyCreateDTO dto)
        {
            var entity = TechCompanyMapper.ToEntity(dto);
            await _techCompanyRepo.AddAsync(entity);
            await _techCompanyRepo.SaveChangesAsync();

            return new GeneralResponse<TechCompanyReadDTO>
            {
                Success = true,
                Message = "TechCompany created successfully.",
                Data = TechCompanyMapper.ToReadDTO(entity)
            };
        }

        public async Task<GeneralResponse<TechCompanyReadDTO>> GetByIdAsync(string id)
        {
            var entity = await _techCompanyRepo.GetByIdWithIncludesAsync(id, t => t.User, t => t.Role);
            if (entity == null)
            {
                return new GeneralResponse<TechCompanyReadDTO>
                {
                    Success = false,
                    Message = "TechCompany not found.",
                    Data = null
                };
            }

            return new GeneralResponse<TechCompanyReadDTO>
            {
                Success = true,
                Message = "Retrieved successfully.",
                Data = TechCompanyMapper.ToReadDTO(entity)
            };
        }

        public async Task<GeneralResponse<IEnumerable<TechCompanyReadDTO>>> GetAllAsync()
        {
            var entities = await _techCompanyRepo.GetAllWithIncludesAsync(t => t.User, t => t.Role);
            return new GeneralResponse<IEnumerable<TechCompanyReadDTO>>
            {
                Success = true,
                Message = "All TechCompanies retrieved successfully.",
                Data = entities.Select(TechCompanyMapper.ToReadDTO)
            };
        }

        public async Task<GeneralResponse<TechCompanyReadDTO>> UpdateAsync(string id, TechCompanyUpdateDTO dto)
        {
            var entity = await _techCompanyRepo.GetByIdAsync(id);
            if (entity == null)
            {
                return new GeneralResponse<TechCompanyReadDTO>
                {
                    Success = false,
                    Message = "TechCompany not found.",
                    Data = null
                };
            }

            TechCompanyMapper.UpdateEntity(entity, dto);
            _techCompanyRepo.Update(entity);
            await _techCompanyRepo.SaveChangesAsync();

            return new GeneralResponse<TechCompanyReadDTO>
            {
                Success = true,
                Message = "Updated successfully.",
                Data = TechCompanyMapper.ToReadDTO(entity)
            };
        }

        public async Task<GeneralResponse<string>> DeleteAsync(string id)
        {
            var entity = await _techCompanyRepo.GetByIdAsync(id);
            if (entity == null)
            {
                return new GeneralResponse<string>
                {
                    Success = false,
                    Message = "TechCompany not found.",
                    Data = null
                };
            }

            _techCompanyRepo.Remove(entity);
            await _techCompanyRepo.SaveChangesAsync();

            return new GeneralResponse<string>
            {
                Success = true,
                Message = "Deleted successfully.",
                Data = id
            };
        }
    }
}
