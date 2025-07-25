using TechpertsSolutions.Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Services;
using Service.Utilities;
using Microsoft.EntityFrameworkCore;
using TechpertsSolutions.Core.Entities;
using Core.DTOs.SpecificationsDTOs;
using Core.DTOs.ProductDTOs;

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
                var specs = await _specRepo.GetAllWithIncludesAsync(s => s.Product);
                return new GeneralResponse<IEnumerable<Core.DTOs.SpecificationsDTOs.SpecificationDTO>>
                {
                    Success = true,
                    Message = "Specifications retrieved successfully.",
                    Data = SpecificationMapper.MapToSpecificationDTOList(specs)
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
            // Input validation
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
                var spec = await _specRepo.GetByIdWithIncludesAsync(id, s => s.Product);
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
            // Input validation
            if (string.IsNullOrWhiteSpace(productId))
            {
                return new GeneralResponse<IEnumerable<Core.DTOs.SpecificationsDTOs.SpecificationDTO>>
                {
                    Success = false,
                    Message = "Product ID cannot be null or empty.",
                    Data = null
                };
            }

            if (!Guid.TryParse(productId, out _))
            {
                return new GeneralResponse<IEnumerable<Core.DTOs.SpecificationsDTOs.SpecificationDTO>>
                {
                    Success = false,
                    Message = "Invalid Product ID format. Expected GUID format.",
                    Data = null
                };
            }

            try
            {
                var productExists = await _productRepo.AnyAsync(p => p.Id == productId);
                if (!productExists)
                {
                    return new GeneralResponse<IEnumerable<Core.DTOs.SpecificationsDTOs.SpecificationDTO>>
                    {
                        Success = false,
                        Message = $"Product with ID '{productId}' not found.",
                        Data = null
                    };
                }

                var specs = await _specRepo.FindWithIncludesAsync(
                    s => s.ProductId == productId,
                    s => s.Product
                );

                return new GeneralResponse<IEnumerable<Core.DTOs.SpecificationsDTOs.SpecificationDTO>>
                {
                    Success = true,
                    Message = "Product specifications retrieved successfully.",
                    Data = SpecificationMapper.MapToSpecificationDTOList(specs)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<Core.DTOs.SpecificationsDTOs.SpecificationDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving product specifications.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<Core.DTOs.SpecificationsDTOs.SpecificationDTO>> CreateSpecificationAsync(CreateSpecificationDTO dto)
        {
            // Input validation
            if (dto == null)
            {
                return new GeneralResponse<Core.DTOs.SpecificationsDTOs.SpecificationDTO>
                {
                    Success = false,
                    Message = "Specification data cannot be null.",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(dto.Key))
            {
                return new GeneralResponse<Core.DTOs.SpecificationsDTOs.SpecificationDTO>
                {
                    Success = false,
                    Message = "Specification key is required.",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(dto.Value))
            {
                return new GeneralResponse<Core.DTOs.SpecificationsDTOs.SpecificationDTO>
                {
                    Success = false,
                    Message = "Specification value is required.",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(dto.ProductId))
            {
                return new GeneralResponse<Core.DTOs.SpecificationsDTOs.SpecificationDTO>
                {
                    Success = false,
                    Message = "Product ID is required.",
                    Data = null
                };
            }

            if (!Guid.TryParse(dto.ProductId, out _))
            {
                return new GeneralResponse<Core.DTOs.SpecificationsDTOs.SpecificationDTO>
                {
                    Success = false,
                    Message = "Invalid Product ID format. Expected GUID format.",
                    Data = null
                };
            }

            try
            {
                var productExists = await _productRepo.AnyAsync(p => p.Id == dto.ProductId);
                if (!productExists)
                {
                    return new GeneralResponse<Core.DTOs.SpecificationsDTOs.SpecificationDTO>
                    {
                        Success = false,
                        Message = $"Product with ID '{dto.ProductId}' not found.",
                        Data = null
                    };
                }

                var spec = SpecificationMapper.MapToSpecification(dto);

                await _specRepo.AddAsync(spec);
                await _specRepo.SaveChangesAsync();

                return new GeneralResponse<Core.DTOs.SpecificationsDTOs.SpecificationDTO>
                {
                    Success = true,
                    Message = "Specification created successfully.",
                    Data = SpecificationMapper.MapToSpecificationDTO(spec)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<Core.DTOs.SpecificationsDTOs.SpecificationDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while creating the specification.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<bool>> UpdateSpecificationAsync(UpdateSpecificationDTO dto)
        {
            // Input validation
            if (dto == null)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Update data cannot be null.",
                    Data = false
                };
            }

            if (string.IsNullOrWhiteSpace(dto.Id))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Specification ID is required.",
                    Data = false
                };
            }

            if (!Guid.TryParse(dto.Id, out _))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Invalid Specification ID format. Expected GUID format.",
                    Data = false
                };
            }

            if (string.IsNullOrWhiteSpace(dto.Key))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Specification key is required.",
                    Data = false
                };
            }

            if (string.IsNullOrWhiteSpace(dto.Value))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Specification value is required.",
                    Data = false
                };
            }

            if (string.IsNullOrWhiteSpace(dto.ProductId))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Product ID is required.",
                    Data = false
                };
            }

            if (!Guid.TryParse(dto.ProductId, out _))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Invalid Product ID format. Expected GUID format.",
                    Data = false
                };
            }

            try
            {
                var spec = await _specRepo.GetByIdAsync(dto.Id);
                if (spec == null)
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = $"Specification with ID '{dto.Id}' not found.",
                        Data = false
                    };
                }

                var productExists = await _productRepo.AnyAsync(p => p.Id == dto.ProductId);
                if (!productExists)
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = $"Product with ID '{dto.ProductId}' not found.",
                        Data = false
                    };
                }

                SpecificationMapper.MapToSpecification(dto, spec);

                _specRepo.Update(spec);
                await _specRepo.SaveChangesAsync();

                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = "Specification updated successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "An unexpected error occurred while updating the specification.",
                    Data = false
                };
            }
        }

        public async Task<GeneralResponse<bool>> DeleteSpecificationAsync(string id)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Specification ID cannot be null or empty.",
                    Data = false
                };
            }

            if (!Guid.TryParse(id, out _))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Invalid Specification ID format. Expected GUID format.",
                    Data = false
                };
            }

            try
            {
                var spec = await _specRepo.GetByIdAsync(id);
                if (spec == null)
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = $"Specification with ID '{id}' not found.",
                        Data = false
                    };
                }

                _specRepo.Remove(spec);
                await _specRepo.SaveChangesAsync();

                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = "Specification deleted successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "An unexpected error occurred while deleting the specification.",
                    Data = false
                };
            }
        }

        public async Task<GeneralResponse<IEnumerable<ProductListItemDTO>>> GetProductsBySpecificationAsync(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
            {
                return new GeneralResponse<IEnumerable<ProductListItemDTO>>
                {
                    Success = false,
                    Message = "Key and value must be provided.",
                    Data = null
                };
            }

            try
            {
                var specs = await _specRepo.FindWithIncludesAsync(
                    s => s.Key == key && s.Value == value,
                    s => s.Product
                );
                var products = specs
                    .Where(s => s.Product != null)
                    .Select(s => s.Product)
                    .Distinct()
                    .Select(ProductMapper.MapToProductListItem)
                    .ToList();
                return new GeneralResponse<IEnumerable<ProductListItemDTO>>
                {
                    Success = true,
                    Message = $"Products with specification {key}={value} retrieved successfully.",
                    Data = products
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<ProductListItemDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving products by specification.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<IEnumerable<ProductListItemDTO>>> GetProductsBySpecificationIdAsync(string specificationId)
        {
            if (string.IsNullOrWhiteSpace(specificationId))
            {
                return new GeneralResponse<IEnumerable<ProductListItemDTO>>
                {
                    Success = false,
                    Message = "Specification ID must be provided.",
                    Data = null
                };
            }
            if (!Guid.TryParse(specificationId, out _))
            {
                return new GeneralResponse<IEnumerable<ProductListItemDTO>>
                {
                    Success = false,
                    Message = "Invalid Specification ID format. Expected GUID format.",
                    Data = null
                };
            }
            try
            {
                var spec = await _specRepo.GetByIdWithIncludesAsync(specificationId, s => s.Product);
                if (spec == null || spec.Product == null)
                {
                    return new GeneralResponse<IEnumerable<ProductListItemDTO>>
                    {
                        Success = false,
                        Message = $"Specification with ID '{specificationId}' not found or has no product.",
                        Data = new List<ProductListItemDTO>()
                    };
                }
                var productList = new List<ProductListItemDTO> { ProductMapper.MapToProductListItem(spec.Product) };
                return new GeneralResponse<IEnumerable<ProductListItemDTO>>
                {
                    Success = true,
                    Message = "Product for the given specification ID retrieved successfully.",
                    Data = productList
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<ProductListItemDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving product by specification ID.",
                    Data = null
                };
            }
        }
    }
}