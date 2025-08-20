using Core.DTOs;
using Core.DTOs.PCAssemblyDTOs;
using Core.DTOs.ProductDTOs;
using Core.Interfaces;
using Core.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using TechpertsSolutions.Core.Entities;

namespace Service
{
    public class PCAssemblyCompatibilityService : IPCAssemblyCompatibilityService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<SubCategory> _subCategoryRepository;
        private readonly IRepository<Specification> _specificationRepository;

        public PCAssemblyCompatibilityService(
            IRepository<Product> productRepository,
            IRepository<Category> categoryRepository,
            IRepository<SubCategory> subCategoryRepository,
            IRepository<Specification> specificationRepository
        )
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _subCategoryRepository = subCategoryRepository;
            _specificationRepository = specificationRepository;
        }

        public async Task<
            GeneralResponse<PaginatedDTO<CompatibleProductDTO>>
        > GetCompatiblePartsAsync(CompatibilityFilterDTO filter, int pageNumber, int pageSize)
        {
            try
            {
                var query = _productRepository.GetAllAsync().Result.AsQueryable();

                // Apply filters
                if (!string.IsNullOrWhiteSpace(filter.CategoryId))
                {
                    query = query.Where(p => p.CategoryId == filter.CategoryId);
                }

                if (!string.IsNullOrWhiteSpace(filter.SubCategoryId))
                {
                    query = query.Where(p => p.SubCategoryId == filter.SubCategoryId);
                }

                if (filter.MaxPrice.HasValue)
                {
                    query = query.Where(p => p.Price <= filter.MaxPrice.Value);
                }

                if (!string.IsNullOrWhiteSpace(filter.TechCompanyId))
                {
                    query = query.Where(p => p.TechCompanyId == filter.TechCompanyId);
                }

                // Only include approved products
                query = query.Where(p => p.status == Core.Enums.ProductPendingStatus.Approved);

                var totalCount = await query.CountAsync();
                var products = await query
                    .Include(p => p.Category)
                    .Include(p => p.SubCategory)
                    .Include(p => p.TechCompany)
                    .Include(p => p.Specifications)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var compatibleProducts = products
                    .Select(p => new CompatibleProductDTO
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        ImageUrl = p.ImageUrl,
                        CategoryName = p.Category?.Name ?? "Unknown",
                        SubCategoryName = p.SubCategory?.Name ?? "Unknown",
                        TechCompanyName = p.TechCompany?.User?.FullName ?? "Unknown",
                        Specifications =
                            p.Specifications?.Select(s => new Core.DTOs.ProductDTOs.SpecificationDTO
                                {
                                    Key = s.Key,
                                    Value = s.Value,
                                })
                                .ToList() ?? new List<Core.DTOs.ProductDTOs.SpecificationDTO>(),
                        IsCompatible = true, // Basic compatibility check - can be enhanced
                        CompatibilityNotes = "Compatible component",
                    })
                    .ToList();

                var paginatedResult = new PaginatedDTO<CompatibleProductDTO>
                {
                    Items = compatibleProducts,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalItems = totalCount,
                };

                return new GeneralResponse<PaginatedDTO<CompatibleProductDTO>>
                {
                    Success = true,
                    Message = "Compatible parts retrieved successfully.",
                    Data = paginatedResult,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<PaginatedDTO<CompatibleProductDTO>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving compatible parts.",
                    Data = null,
                };
            }
        }

        public async Task<
            GeneralResponse<PaginatedDTO<CompatibleProductDTO>>
        > GetComponentsByFilterAsync(CompatibilityFilterDTO filter, int pageNumber, int pageSize)
        {
            try
            {
                var query = _productRepository.GetAllAsync().Result.AsQueryable();

                // Apply filters
                if (!string.IsNullOrWhiteSpace(filter.ComponentId))
                {
                    // Get the component to check compatibility with
                    var component = await _productRepository.GetByIdAsync(filter.ComponentId);
                    if (component != null)
                    {
                        // Apply compatibility logic based on component type
                        query = ApplyCompatibilityFilters(query, component);
                    }
                }

                if (!string.IsNullOrWhiteSpace(filter.CategoryId))
                {
                    query = query.Where(p => p.CategoryId == filter.CategoryId);
                }

                if (!string.IsNullOrWhiteSpace(filter.SubCategoryId))
                {
                    query = query.Where(p => p.SubCategoryId == filter.SubCategoryId);
                }

                if (filter.MaxPrice.HasValue)
                {
                    query = query.Where(p => p.Price <= filter.MaxPrice.Value);
                }

                if (!string.IsNullOrWhiteSpace(filter.TechCompanyId))
                {
                    query = query.Where(p => p.TechCompanyId == filter.TechCompanyId);
                }

                // Only include approved products
                query = query.Where(p => p.status == Core.Enums.ProductPendingStatus.Approved);

                var totalCount = await query.CountAsync();
                var products = await query
                    .Include(p => p.Category)
                    .Include(p => p.SubCategory)
                    .Include(p => p.TechCompany)
                    .Include(p => p.Specifications)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var compatibleProducts = products
                    .Select(p => new CompatibleProductDTO
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        ImageUrl = p.ImageUrl,
                        CategoryName = p.Category?.Name ?? "Unknown",
                        SubCategoryName = p.SubCategory?.Name ?? "Unknown",
                        TechCompanyName = p.TechCompany?.User?.FullName ?? "Unknown",
                        Specifications =
                            p.Specifications?.Select(s => new Core.DTOs.ProductDTOs.SpecificationDTO
                                {
                                    Key = s.Key,
                                    Value = s.Value,
                                })
                                .ToList() ?? new List<Core.DTOs.ProductDTOs.SpecificationDTO>(),
                        IsCompatible = true,
                        CompatibilityNotes = "Compatible with selected component",
                    })
                    .ToList();

                var paginatedResult = new PaginatedDTO<CompatibleProductDTO>
                {
                    Items = compatibleProducts,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalItems = totalCount,
                };

                return new GeneralResponse<PaginatedDTO<CompatibleProductDTO>>
                {
                    Success = true,
                    Message = "Components retrieved successfully.",
                    Data = paginatedResult,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<PaginatedDTO<CompatibleProductDTO>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving components.",
                    Data = null,
                };
            }
        }

        public async Task<
            GeneralResponse<PaginatedDTO<CompatibleProductDTO>>
        > GetCompatiblePartsByCategoryAsync(string categoryId, int pageNumber, int pageSize)
        {
            try
            {
                var filter = new CompatibilityFilterDTO { CategoryId = categoryId };
                return await GetCompatiblePartsAsync(filter, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                return new GeneralResponse<PaginatedDTO<CompatibleProductDTO>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving compatible parts by category.",
                    Data = null,
                };
            }
        }

        public async Task<GeneralResponse<bool>> CheckCompatibilityAsync(
            string productId1,
            string productId2
        )
        {
            try
            {
                var product1 = await _productRepository.GetByIdAsync(productId1);
                var product2 = await _productRepository.GetByIdAsync(productId2);

                if (product1 == null || product2 == null)
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "One or both products not found.",
                    };
                }

                // Basic compatibility check - can be enhanced with more sophisticated logic
                var isCompatible = CheckBasicCompatibility(product1, product2);

                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = isCompatible
                        ? "Products are compatible."
                        : "Products are not compatible.",
                    Data = isCompatible,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "An error occurred while checking compatibility.",
                    Data = false,
                };
            }
        }

        private IQueryable<Product> ApplyCompatibilityFilters(
            IQueryable<Product> query,
            Product component
        )
        {
            // This is a basic implementation - can be enhanced with more sophisticated compatibility logic
            // For now, we'll just filter by different categories to avoid conflicts

            // Example: If component is a CPU, exclude other CPUs
            if (
                component.SubCategory?.Name?.Contains("CPU", StringComparison.OrdinalIgnoreCase)
                == true
            )
            {
                query = query.Where(p =>
                    !p.SubCategory.Name.Contains("CPU", StringComparison.OrdinalIgnoreCase)
                );
            }

            // Example: If component is a motherboard, exclude other motherboards
            if (
                component.SubCategory?.Name?.Contains(
                    "Motherboard",
                    StringComparison.OrdinalIgnoreCase
                ) == true
            )
            {
                query = query.Where(p =>
                    !p.SubCategory.Name.Contains("Motherboard", StringComparison.OrdinalIgnoreCase)
                );
            }

            return query;
        }

        private bool CheckBasicCompatibility(Product product1, Product product2)
        {
            // Basic compatibility check - can be enhanced with more sophisticated logic
            // For now, we'll just check if they're in different categories to avoid conflicts

            if (
                product1.CategoryId == product2.CategoryId
                && product1.SubCategoryId == product2.SubCategoryId
            )
            {
                // Same category and subcategory - likely not compatible (e.g., two CPUs)
                return false;
            }

            // Additional compatibility checks can be added here based on specifications
            // For example, checking socket compatibility for CPU and motherboard

            return true;
        }
    }
}
