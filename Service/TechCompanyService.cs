using Core.DTOs.TechCompanyDTOs;
using Core.Interfaces;
using Core.Interfaces.Services;
using Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTOs;
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
            
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<TechCompanyReadDTO>
                {
                    Success = false,
                    Message = "TechCompany ID cannot be null or empty.",
                    Data = null
                };
            }

            if (!Guid.TryParse(id, out _))
            {
                return new GeneralResponse<TechCompanyReadDTO>
                {
                    Success = false,
                    Message = "Invalid TechCompany ID format. Expected GUID format.",
                    Data = null
                };
            }

            try
            {
                // Comprehensive includes for detailed tech company view with user and role information
                var entity = await _techCompanyRepo.GetByIdWithIncludesAsync(id, 
                    t => t.User, 
                    t => t.Role);

                if (entity == null)
                {
                    return new GeneralResponse<TechCompanyReadDTO>
                    {
                        Success = false,
                        Message = $"TechCompany with ID '{id}' not found.",
                        Data = null
                    };
                }

                return new GeneralResponse<TechCompanyReadDTO>
                {
                    Success = true,
                    Message = "TechCompany retrieved successfully.",
                    Data = TechCompanyMapper.MapToTechCompanyReadDTO(entity)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<TechCompanyReadDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving the tech company.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<IEnumerable<TechCompanyReadDTO>>> GetAllAsync()
        {
            try
            {
                // Optimized includes for tech company listing with user and role information
                var entities = await _techCompanyRepo.GetAllWithIncludesAsync(
                    t => t.User, 
                    t => t.Role);

                var techCompanyDtos = entities.Select(TechCompanyMapper.MapToTechCompanyReadDTO).ToList();

                return new GeneralResponse<IEnumerable<TechCompanyReadDTO>>
                {
                    Success = true,
                    Message = "TechCompanies retrieved successfully.",
                    Data = techCompanyDtos
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<TechCompanyReadDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving tech companies.",
                    Data = null
                };
            }
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
