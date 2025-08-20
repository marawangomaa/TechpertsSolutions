using Core.DTOs;
using Core.DTOs.TechCompanyDTOs;
using Core.Interfaces;
using Core.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using Service.Utilities;
using TechpertsSolutions.Core.Entities;
using TechpertsSolutions.Repository.Data;

namespace Service
{
    public class TechCompanyService : ITechCompanyService
    {
        private readonly IRepository<TechCompany> _techCompanyRepo;
        private readonly TechpertsContext context;

        public TechCompanyService(IRepository<TechCompany> repo, TechpertsContext _context)
        {
            _techCompanyRepo = repo;
            context = _context;
        }

        public async Task<GeneralResponse<TechCompanyReadDTO>> UpdateAsync(
            string id,
            TechCompanyUpdateDTO dto
        )
        {
            if (string.IsNullOrWhiteSpace(id) || dto == null)
            {
                return new GeneralResponse<TechCompanyReadDTO>
                {
                    Success = false,
                    Message = "Invalid input data.",
                    Data = null,
                };
            }

            var entity = await _techCompanyRepo.GetByIdAsync(id);
            if (entity == null)
            {
                return new GeneralResponse<TechCompanyReadDTO>
                {
                    Success = false,
                    Message = "TechCompany not found.",
                    Data = null,
                };
            }

            TechCompanyMapper.UpdateEntity(entity, dto);
            _techCompanyRepo.Update(entity);
            await _techCompanyRepo.SaveChangesAsync();

            // Reload updated entity with includes
            var updatedEntity = await _techCompanyRepo.GetByIdWithIncludesAsync(
                id,
                t => t.User,
                t => t.Role
            );

            return new GeneralResponse<TechCompanyReadDTO>
            {
                Success = true,
                Message = "Updated successfully.",
                Data = TechCompanyMapper.ToReadDTO(updatedEntity),
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
                    Data = null,
                };
            }

            if (!Guid.TryParse(id, out _))
            {
                return new GeneralResponse<TechCompanyReadDTO>
                {
                    Success = false,
                    Message = "Invalid TechCompany ID format. Expected GUID format.",
                    Data = null,
                };
            }

            try
            {
                // Comprehensive includes for detailed tech company view with user and role information
                var entity = await _techCompanyRepo.GetByIdWithIncludesAsync(
                    id,
                    t => t.User,
                    t => t.Role
                );

                if (entity == null)
                {
                    return new GeneralResponse<TechCompanyReadDTO>
                    {
                        Success = false,
                        Message = $"TechCompany with ID '{id}' not found.",
                        Data = null,
                    };
                }

                return new GeneralResponse<TechCompanyReadDTO>
                {
                    Success = true,
                    Message = "TechCompany retrieved successfully.",
                    Data = TechCompanyMapper.MapToTechCompanyReadDTO(entity),
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<TechCompanyReadDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving the tech company.",
                    Data = null,
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
                    t => t.Role
                );

                var techCompanyDtos = entities
                    .Select(TechCompanyMapper.MapToTechCompanyReadDTO)
                    .ToList();

                return new GeneralResponse<IEnumerable<TechCompanyReadDTO>>
                {
                    Success = true,
                    Message = "TechCompanies retrieved successfully.",
                    Data = techCompanyDtos,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<TechCompanyReadDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving tech companies.",
                    Data = null,
                };
            }
        }

        public async Task<GeneralResponse<TechCompanyReadDTO>> GetByUserId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<TechCompanyReadDTO>
                {
                    Success = false,
                    Message = "TechCompany ID cannot be null or empty.",
                    Data = null,
                };
            }

            if (!Guid.TryParse(id, out _))
            {
                return new GeneralResponse<TechCompanyReadDTO>
                {
                    Success = false,
                    Message = "Invalid TechCompany ID format. Expected GUID format.",
                    Data = null,
                };
            }

            try
            {

                var entity = await _techCompanyRepo.GetByIdWithIncludesAsync(
                    id,
                    t => t.User,
                    t => t.Role
                );


                if (entity == null)
                {
                    return new GeneralResponse<TechCompanyReadDTO>
                    {
                        Success = false,
                        Message = $"TechCompany with ID '{id}' not found.",
                        Data = null,
                    };
                }

                return new GeneralResponse<TechCompanyReadDTO>
                {
                    Success = true,
                    Message = "TechCompany retrieved successfully.",
                    Data = TechCompanyMapper.MapToTechCompanyReadDTO(entity),
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<TechCompanyReadDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving the tech company.",
                    Data = null,
                };
            }
        }

        public async Task<GeneralResponse<TechCompanyReadDTO>> UpdateRatingAsync(string id, decimal rating)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<TechCompanyReadDTO>
                {
                    Success = false,
                    Message = "Invalid ID.",
                    Data = null,
                };
            }

            var entity = await _techCompanyRepo.GetByIdAsync(id);
            if (entity == null)
            {
                return new GeneralResponse<TechCompanyReadDTO>
                {
                    Success = false,
                    Message = "TechCompany not found.",
                    Data = null,
                };
            }

            // Update rating only
            entity.Rating = rating;

            _techCompanyRepo.Update(entity);
            await _techCompanyRepo.SaveChangesAsync();

            // Reload with includes
            var updatedEntity = await _techCompanyRepo.GetByIdWithIncludesAsync(
                id,
                t => t.User,
                t => t.Role
            );

            return new GeneralResponse<TechCompanyReadDTO>
            {
                Success = true,
                Message = "Rating updated successfully.",
                Data = TechCompanyMapper.ToReadDTO(updatedEntity),
            };
        }

        public async Task CleanupTechCompanyDataAsync(string userId)
        {
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                // 1. Get TechCompany by userId with optional includes
                var techCompany = await _techCompanyRepo.GetFirstOrDefaultWithIncludesAsync(tc =>
                    tc.UserId == userId
                );

                if (techCompany == null)
                {
                    await transaction.CommitAsync(); // No cleanup needed
                    return;
                }

                string techCompanyId = techCompany.Id;

                // 2. Find all products for this TechCompany
                var products = await context
                    .Products.Where(p => p.TechCompanyId == techCompanyId)
                    .ToListAsync();

                // 3. Find and remove all PCAssemblyItems that reference these products
                var productIds = products.Select(p => p.Id).ToList();

                var assemblyItems = await context
                    .PCAssemblyItems.Where(ai => productIds.Contains(ai.ProductId))
                    .ToListAsync();

                context.PCAssemblyItems.RemoveRange(assemblyItems);

                // 4. Remove all the products
                context.Products.RemoveRange(products);

                // 5. Remove TechCompany
                _techCompanyRepo.Remove(techCompany);

                // 6. Save all changes and commit transaction
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
