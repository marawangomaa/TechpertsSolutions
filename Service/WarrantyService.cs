using Core.DTOs.WarrantyDTOs;
using Core.DTOs;
using TechpertsSolutions.Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Services;
using Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class WarrantyService : IWarrantyService
    {
        private readonly IRepository<Warranty> _warrantyRepo;
        private readonly IRepository<Product> _productRepo;

        public WarrantyService(IRepository<Warranty> warrantyRepo, IRepository<Product> productRepo)
        {
            _warrantyRepo = warrantyRepo;
            _productRepo = productRepo;
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
                Type = dto.Type,
                Duration = dto.Duration,
                Description = dto.Description,
                ProductId = dto.ProductId,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(int.Parse(dto.Duration))
            };

            await _warrantyRepo.AddAsync(warranty);
            await _warrantyRepo.SaveChangesAsync();

            return new GeneralResponse<WarrantyReadDTO>
            {
                Success = true,
                Message = "Warranty created successfully.",
                Data = WarrantyMapper.MapToWarrantyReadDTO(warranty)
            };
        }

        public async Task<GeneralResponse<WarrantyReadDTO>> GetByIdAsync(string id)
        {
            
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<WarrantyReadDTO>
                {
                    Success = false,
                    Message = "Warranty ID cannot be null or empty.",
                    Data = null
                };
            }

            if (!Guid.TryParse(id, out _))
            {
                return new GeneralResponse<WarrantyReadDTO>
                {
                    Success = false,
                    Message = "Invalid Warranty ID format. Expected GUID format.",
                    Data = null
                };
            }

            try
            {
                // Comprehensive includes for detailed warranty view with product information
                var warranty = await _warrantyRepo.GetByIdWithIncludesAsync(id, 
                    w => w.Product,
                    w => w.Product.Category,
                    w => w.Product.SubCategory,
                    w => w.Product.TechCompany);

                if (warranty == null)
                {
                    return new GeneralResponse<WarrantyReadDTO>
                    {
                        Success = false,
                        Message = $"Warranty with ID '{id}' not found.",
                        Data = null
                    };
                }

                return new GeneralResponse<WarrantyReadDTO>
                {
                    Success = true,
                    Message = "Warranty retrieved successfully.",
                    Data = WarrantyMapper.MapToWarrantyReadDTO(warranty)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<WarrantyReadDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving the warranty.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<IEnumerable<WarrantyReadDTO>>> GetAllAsync()
        {
            try
            {
                // Optimized includes for warranty listing with product information
                var warranties = await _warrantyRepo.GetAllWithIncludesAsync(
                    w => w.Product,
                    w => w.Product.Category,
                    w => w.Product.SubCategory,
                    w => w.Product.TechCompany);

                var warrantyDtos = warranties.Select(WarrantyMapper.MapToWarrantyReadDTO).ToList();

                return new GeneralResponse<IEnumerable<WarrantyReadDTO>>
                {
                    Success = true,
                    Message = "Warranties retrieved successfully.",
                    Data = warrantyDtos
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<WarrantyReadDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving warranties.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<WarrantyReadDTO>> UpdateAsync(string id, WarrantyUpdateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(id) || dto == null)
            {
                return new GeneralResponse<WarrantyReadDTO>
                {
                    Success = false,
                    Message = "Invalid input data.",
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

            warranty.Type = dto.Type;
            warranty.Duration = dto.Duration;
            warranty.Description = dto.Description;
            if (dto.Duration != null && int.TryParse(dto.Duration, out int duration))
                warranty.EndDate = warranty.StartDate.AddMonths(duration);

            _warrantyRepo.Update(warranty);
            await _warrantyRepo.SaveChangesAsync();

            return new GeneralResponse<WarrantyReadDTO>
            {
                Success = true,
                Message = "Warranty updated successfully.",
                Data = WarrantyMapper.MapToWarrantyReadDTO(warranty)
            };
        }

        public async Task<GeneralResponse<bool>> DeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Warranty ID cannot be null or empty.",
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
