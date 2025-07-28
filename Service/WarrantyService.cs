using Core.DTOs.WarrantyDTOs;
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
    public class WarrantyService : IWarrantyService
    {
        private readonly IRepository<Warranty> _warrantyRepo;

        public WarrantyService(IRepository<Warranty> warrantyRepo)
        {
            _warrantyRepo = warrantyRepo;
        }

        public async Task<GeneralResponse<WarrantyReadDTO>> CreateAsync(WarrantyCreateDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ProductId))
            {
                return new GeneralResponse<WarrantyReadDTO>
                {
                    Success = false,
                    Message = "Invalid warranty data.",
                    Data = null
                };
            }

            var warranty = new Warranty
            {
                Id = Guid.NewGuid().ToString(),
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                ProductId = dto.ProductId
            };

            await _warrantyRepo.AddAsync(warranty);
            await _warrantyRepo.SaveChangesAsync();

            return new GeneralResponse<WarrantyReadDTO>
            {
                Success = true,
                Message = "Warranty created successfully.",
                Data = new WarrantyReadDTO
                {
                    Id = warranty.Id,
                    Description = warranty.Description,
                    StartDate = warranty.StartDate,
                    EndDate = warranty.EndDate,
                    ProductId = warranty.ProductId
                }
            };
        }

        public async Task<GeneralResponse<WarrantyReadDTO>> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<WarrantyReadDTO>
                {
                    Success = false,
                    Message = "ID cannot be null or empty.",
                    Data = null
                };
            }

            var warranty = await _warrantyRepo.GetByIdWithIncludesAsync(id, w => w.Product);

            if (warranty == null)
            {
                return new GeneralResponse<WarrantyReadDTO>
                {
                    Success = false,
                    Message = "Warranty not found.",
                    Data = null
                };
            }

            return new GeneralResponse<WarrantyReadDTO>
            {
                Success = true,
                Message = "Warranty retrieved successfully.",
                Data = new WarrantyReadDTO
                {
                    Id = warranty.Id,
                    Description = warranty.Description,
                    StartDate = warranty.StartDate,
                    EndDate = warranty.EndDate,
                    ProductId = warranty.ProductId
                }
            };
        }

        public async Task<GeneralResponse<IEnumerable<WarrantyReadDTO>>> GetAllAsync()
        {
            var warranties = await _warrantyRepo.GetAllWithIncludesAsync(w => w.Product);

            return new GeneralResponse<IEnumerable<WarrantyReadDTO>>
            {
                Success = true,
                Message = "Warranties retrieved successfully.",
                Data = warranties.Select(w => new WarrantyReadDTO
                {
                    Id = w.Id,
                    Description = w.Description,
                    StartDate = w.StartDate,
                    EndDate = w.EndDate,
                    ProductId = w.ProductId
                })
            };
        }

        public async Task<GeneralResponse<WarrantyReadDTO>> UpdateAsync(string id, WarrantyUpdateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(id) || dto == null)
            {
                return new GeneralResponse<WarrantyReadDTO>
                {
                    Success = false,
                    Message = "Invalid input.",
                    Data = null
                };
            }

            var warranty = await _warrantyRepo.GetByIdAsync(id);
            if (warranty == null)
            {
                return new GeneralResponse<WarrantyReadDTO>
                {
                    Success = false,
                    Message = "Warranty not found.",
                    Data = null
                };
            }

            if (!string.IsNullOrWhiteSpace(dto.Description)) warranty.Description = dto.Description;
            if (dto.StartDate.HasValue) warranty.StartDate = dto.StartDate.Value;
            if (dto.EndDate.HasValue) warranty.EndDate = dto.EndDate.Value;
            if (!string.IsNullOrWhiteSpace(dto.ProductId)) warranty.ProductId = dto.ProductId;

            _warrantyRepo.Update(warranty);
            await _warrantyRepo.SaveChangesAsync();

            return new GeneralResponse<WarrantyReadDTO>
            {
                Success = true,
                Message = "Warranty updated successfully.",
                Data = new WarrantyReadDTO
                {
                    Id = warranty.Id,
                    Description = warranty.Description,
                    StartDate = warranty.StartDate,
                    EndDate = warranty.EndDate,
                    ProductId = warranty.ProductId
                }
            };
        }

        public async Task<GeneralResponse<bool>> DeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "ID cannot be null or empty.",
                    Data = false
                };
            }

            var warranty = await _warrantyRepo.GetByIdAsync(id);
            if (warranty == null)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Warranty not found.",
                    Data = false
                };
            }

            _warrantyRepo.Remove(warranty);
            await _warrantyRepo.SaveChangesAsync();

            return new GeneralResponse<bool>
            {
                Success = true,
                Message = "Warranty deleted successfully.",
                Data = true
            };
        }
    }
}
