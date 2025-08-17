using Core.DTOs;
using Core.DTOs.SpecificationsDTOs;
using Core.DTOs.ProductDTOs;
using TechpertsSolutions.Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Services;
using Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;

namespace Service
{
    public class SpecificationService : ISpecificationService
    {
        private readonly IRepository<Specification> _specRepo;
        private readonly IRepository<Product> _productRepo;

        public SpecificationService(IRepository<Specification> specRepo, IRepository<Product> productRepo)
        {
            _specRepo = specRepo;
            _productRepo = productRepo;
        }

        public async Task<GeneralResponse<IEnumerable<Core.DTOs.SpecificationsDTOs.SpecificationDTO>>> GetAllSpecificationsAsync()
        {
            try
            {
                // Optimized includes for specification listing with product information
                var specs = await _specRepo.GetAllWithIncludesAsync(
                    s => s.Product,
                    s => s.Product.Category,
                    s => s.Product.SubCategory,
                    s => s.Product.TechCompany,
                    s => s.Product.Warranties);

                var specDtos = specs.Select(SpecificationMapper.MapToSpecificationDTO).ToList();

                return new GeneralResponse<IEnumerable<Core.DTOs.SpecificationsDTOs.SpecificationDTO>>
                {
                    Success = true,
                    Message = "Specifications retrieved successfully.",
                    Data = specDtos
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<Core.DTOs.SpecificationsDTOs.SpecificationDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving specifications.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<Core.DTOs.SpecificationsDTOs.SpecificationDTO>> GetSpecificationByIdAsync(string id)
        {
            
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<Core.DTOs.SpecificationsDTOs.SpecificationDTO>
                {
                    Success = false,
                    Message = "Specification ID cannot be null or empty.",
                    Data = null
                };
            }

            if (!Guid.TryParse(id, out _))
            {
                return new GeneralResponse<Core.DTOs.SpecificationsDTOs.SpecificationDTO>
                {
                    Success = false,
                    Message = "Invalid Specification ID format. Expected GUID format.",
                    Data = null
                };
            }

            try
            {
                // Comprehensive includes for detailed specification view with product information
                var spec = await _specRepo.GetByIdWithIncludesAsync(id, 
                    s => s.Product,
                    s => s.Product.Category,
                    s => s.Product.SubCategory,
                    s => s.Product.TechCompany,
                    s => s.Product.Warranties);

                if (spec == null)
                {
                    return new GeneralResponse<Core.DTOs.SpecificationsDTOs.SpecificationDTO>
                    {
                        Success = false,
                        Message = $"Specification with ID '{id}' not found.",
                        Data = null
                    };
                }

                return new GeneralResponse<Core.DTOs.SpecificationsDTOs.SpecificationDTO>
                {
                    Success = true,
                    Message = "Specification retrieved successfully.",
                    Data = SpecificationMapper.MapToSpecificationDTO(spec)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<Core.DTOs.SpecificationsDTOs.SpecificationDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving the specification.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<IEnumerable<Core.DTOs.SpecificationsDTOs.SpecificationDTO>>> GetSpecificationsByProductIdAsync(string productId)
        {
            if (string.IsNullOrWhiteSpace(productId))
            {
                return new GeneralResponse<IEnumerable<Core.DTOs.SpecificationsDTOs.SpecificationDTO>>
                {
                    Success = false,
                    Message = "Product ID cannot be null or empty.",
                    Data = null
                };
            }
            
            var specs = await _specRepo.FindAsync(s => s.ProductId == productId,s => s.Product);
            var specDtos = specs.Select(SpecificationMapper.MapToSpecificationDTO).ToList();

            return new GeneralResponse<IEnumerable<Core.DTOs.SpecificationsDTOs.SpecificationDTO>>
            {
                Success = true,
                Message = "Product specifications retrieved successfully.",
                Data = specDtos
            };
        }

        public async Task<GeneralResponse<Core.DTOs.SpecificationsDTOs.SpecificationDTO>> CreateSpecificationAsync(CreateSpecificationDTO createDto)
        {
            if (createDto == null || string.IsNullOrWhiteSpace(createDto.ProductId))
            {
                return new GeneralResponse<Core.DTOs.SpecificationsDTOs.SpecificationDTO>
                {
                    Success = false,
                    Message = "Invalid specification data.",
                    Data = null
                };
            }

            var product = await _productRepo.GetByIdAsync(createDto.ProductId);
            if (product == null)
            {
                return new GeneralResponse<Core.DTOs.SpecificationsDTOs.SpecificationDTO>
                {
                    Success = false,
                    Message = "Product not found.",
                    Data = null
                };
            }

            var specification = new Specification
            {
                Key = createDto.Key,
                Value = createDto.Value,
                ProductId = createDto.ProductId
            };

            await _specRepo.AddAsync(specification);
            await _specRepo.SaveChangesAsync();

            return new GeneralResponse<Core.DTOs.SpecificationsDTOs.SpecificationDTO>
            {
                Success = true,
                Message = "Specification created successfully.",
                Data = SpecificationMapper.MapToSpecificationDTO(specification)
            };
        }

        public async Task<GeneralResponse<bool>> UpdateSpecificationAsync(UpdateSpecificationDTO updateDto)
        {
            if (updateDto == null || string.IsNullOrWhiteSpace(updateDto.Id))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Invalid specification data.",
                    Data = false
                };
            }

            var specification = await _specRepo.GetByIdAsync(updateDto.Id);
            if (specification == null)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Specification not found.",
                    Data = false
                };
            }

            specification.Key = updateDto.Key;
            specification.Value = updateDto.Value;

            _specRepo.Update(specification);
            await _specRepo.SaveChangesAsync();

            return new GeneralResponse<bool>
            {
                Success = true,
                Message = "Specification updated successfully.",
                Data = true
            };
        }

        public async Task<GeneralResponse<bool>> DeleteSpecificationAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Specification ID cannot be null or empty.",
                    Data = false
                };
            }

            var specification = await _specRepo.GetByIdAsync(id);
            if (specification == null)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Specification not found.",
                    Data = false
                };
            }

            _specRepo.Remove(specification);
            await _specRepo.SaveChangesAsync();

            return new GeneralResponse<bool>
            {
                Success = true,
                Message = "Specification deleted successfully.",
                Data = true
            };
        }

        public async Task<GeneralResponse<IEnumerable<ProductListItemDTO>>> GetProductsBySpecificationAsync(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
            {
                return new GeneralResponse<IEnumerable<ProductListItemDTO>>
                {
                    Success = false,
                    Message = "Key and value cannot be null or empty.",
                    Data = null
                };
            }

            var specifications = await _specRepo.FindAsync(s => s.Key == key && s.Value == value);
            var productIds = specifications.Select(s => s.ProductId).Distinct().ToList();

            var products = await _productRepo.FindAsync(p => productIds.Contains(p.Id));
            var productDtos = products.Select(p => new ProductListItemDTO
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                ImageUrl = p.ImageUrl
            }).ToList();

            return new GeneralResponse<IEnumerable<ProductListItemDTO>>
            {
                Success = true,
                Message = "Products found by specification.",
                Data = productDtos
            };
        }

        public async Task<GeneralResponse<IEnumerable<ProductListItemDTO>>> GetProductsBySpecificationIdAsync(string specificationId)
        {
            if (string.IsNullOrWhiteSpace(specificationId))
            {
                return new GeneralResponse<IEnumerable<ProductListItemDTO>>
                {
                    Success = false,
                    Message = "Specification ID cannot be null or empty.",
                    Data = null
                };
            }

            var specification = await _specRepo.GetByIdAsync(specificationId);
            if (specification == null)
            {
                return new GeneralResponse<IEnumerable<ProductListItemDTO>>
                {
                    Success = false,
                    Message = "Specification not found.",
                    Data = null
                };
            }

            var specifications = await _specRepo.FindAsync(s => s.Key == specification.Key && s.Value == specification.Value);
            var productIds = specifications.Select(s => s.ProductId).Distinct().ToList();

            var products = await _productRepo.FindAsync(p => productIds.Contains(p.Id));
            var productDtos = products.Select(p => new ProductListItemDTO
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                ImageUrl = p.ImageUrl
            }).ToList();

            return new GeneralResponse<IEnumerable<ProductListItemDTO>>
            {
                Success = true,
                Message = "Products found by specification.",
                Data = productDtos
            };
        }
    }
}
