using Core.DTOs.ProductDTOs;
using TechpertsSolutions.Core.DTOs;
using Core.Enums;
using Core.Interfaces;
using TechpertsSolutions.Core.Entities;
using Core.Interfaces.Services;
using Service.Utilities;
using System.Linq;
using Core.Utilities;
using Microsoft.AspNetCore.Http;
using Core.DTOs;

namespace Service
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<Specification> _specRepo;
        private readonly IRepository<Warranty> _warrantyRepo;
        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<SubCategory> _subCategoryRepo;
        private readonly IRepository<TechCompany> _techCompanyRepo;
        private readonly IFileService _fileService;
        private readonly INotificationService _notificationService;

        public ProductService(IRepository<Product> productRepo,
            IRepository<Specification> specRepo,
            IRepository<Warranty> warrantyRepo,
            IRepository<Category> categoryRepo,
            IRepository<SubCategory> subCategoryRepo,
            IRepository<TechCompany> techCompanyRepo,
            IFileService fileService,
            INotificationService notificationService)
        {
            _productRepo = productRepo;
            _specRepo = specRepo;
            _warrantyRepo = warrantyRepo;
            _categoryRepo = categoryRepo;
            _subCategoryRepo = subCategoryRepo;
            _techCompanyRepo = techCompanyRepo;
            _fileService = fileService;
            _notificationService = notificationService;
        }

        public async Task<GeneralResponse<PaginatedDTO<ProductCardDTO>>> GetAllAsync(
            int pageNumber = 1,
            int pageSize = 10,
            ProductPendingStatus? status = null,
            ProductCategory? categoryEnum = null,
            string? subCategoryName = null,
            string? nameSearch = null,
            string? sortBy = null,
            bool sortDescending = false)
        {
            
            if (pageNumber < 1)
            {
                return new GeneralResponse<PaginatedDTO<ProductCardDTO>>
                {
                    Success = false,
                    Message = "Page number must be greater than 0.",
                    Data = null
                };
            }

            if (pageSize < 1 || pageSize > 100)
            {
                return new GeneralResponse<PaginatedDTO<ProductCardDTO>>
                {
                    Success = false,
                    Message = "Page size must be between 1 and 100.",
                    Data = null
                };
            }

            try
            {
                // Optimized includes for product listing - include all needed for ProductCardDTO
                var allProducts = (await _productRepo.GetAllWithIncludesAsync(
                    p => p.Category, 
                    p => p.SubCategory, 
                    p => p.TechCompany,
                    p => p.Specifications,
                    p => p.Warranties)).AsQueryable();

                
                if (status.HasValue)
                    allProducts = allProducts.Where(p => p.status == status.Value);

                if (categoryEnum.HasValue)
                {
                    var categoryName = categoryEnum.Value.GetStringValue();
                    allProducts = allProducts.Where(p => p.Category != null && p.Category.Name == categoryName);
                }

                if (!string.IsNullOrWhiteSpace(subCategoryName))
                {
                    allProducts = allProducts.Where(p => p.SubCategory != null && p.SubCategory.Name == subCategoryName);
                }

                if (!string.IsNullOrWhiteSpace(nameSearch))
                    allProducts = allProducts.Where(p => p.Name.Contains(nameSearch, StringComparison.OrdinalIgnoreCase));

                
                allProducts = sortBy?.ToLower() switch
                {
                    "price" => sortDescending ? allProducts.OrderByDescending(p => p.Price) : allProducts.OrderBy(p => p.Price),
                    "name" => sortDescending ? allProducts.OrderByDescending(p => p.Name) : allProducts.OrderBy(p => p.Name),
                    "stock" => sortDescending ? allProducts.OrderByDescending(p => p.Stock) : allProducts.OrderBy(p => p.Stock),
                    "createdat" => sortDescending ? allProducts.OrderByDescending(p => p.CreatedAt) : allProducts.OrderBy(p => p.CreatedAt),
                    _ => sortDescending ? allProducts.OrderByDescending(p => p.Name) : allProducts.OrderBy(p => p.Name)
                };

                var totalCount = allProducts.Count();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var products = allProducts
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var productDtos = products.Select(ProductMapper.MapToProductCard).ToList();

                var paginatedResult = new PaginatedDTO<ProductCardDTO>
                {
                    Items = productDtos,
                    TotalItems = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return new GeneralResponse<PaginatedDTO<ProductCardDTO>>
                {
                    Success = true,
                    Message = "Products retrieved successfully.",
                    Data = paginatedResult
                };
            }
            catch (Exception)
            {
                return new GeneralResponse<PaginatedDTO<ProductCardDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving products.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<ProductDTO>> GetByIdAsync(string id)
        {
            
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<ProductDTO>
                {
                    Success = false,
                    Message = "Product ID cannot be null or empty.",
                    Data = null
                };
            }

            if (!Guid.TryParse(id, out _))
            {
                return new GeneralResponse<ProductDTO>
                {
                    Success = false,
                    Message = "Invalid Product ID format. Expected GUID format.",
                    Data = null
                };
            }

            try
            {
                // Comprehensive includes for detailed product view
                var product = await _productRepo.GetByIdWithIncludesAsync(id,
                    p => p.Category,
                    p => p.SubCategory,
                    p => p.TechCompany,
                    p => p.TechCompany.User,
                    p => p.Specifications,
                    p => p.Warranties);

                if (product == null)
                {
                    return new GeneralResponse<ProductDTO>
                    {
                        Success = false,
                        Message = $"Product with ID '{id}' not found.",
                        Data = null
                    };
                }

                return new GeneralResponse<ProductDTO>
                {
                    Success = true,
                    Message = "Product retrieved successfully.",
                    Data = ProductMapper.MapToProductDTO(product)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<ProductDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving the product.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<ProductDTO>> AddAsync(ProductCreateDTO dto, ProductCategory category, ProductPendingStatus status)
        {
            
            if (dto == null)
            {
                return new GeneralResponse<ProductDTO>
                {
                    Success = false,
                    Message = "Product data cannot be null.",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return new GeneralResponse<ProductDTO>
                {
                    Success = false,
                    Message = "Product name is required.",
                    Data = null
                };
            }

            if (dto.Price <= 0)
            {
                return new GeneralResponse<ProductDTO>
                {
                    Success = false,
                    Message = "Product price must be greater than 0.",
                    Data = null
                };
            }

            if (dto.Stock < 0)
            {
                return new GeneralResponse<ProductDTO>
                {
                    Success = false,
                    Message = "Product stock cannot be negative.",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(dto.TechCompanyId))
            {
                return new GeneralResponse<ProductDTO>
                {
                    Success = false,
                    Message = "Tech Company ID is required.",
                    Data = null
                };
            }

            // Validate TechCompany ID format
            if (!Guid.TryParse(dto.TechCompanyId, out _))
            {
                return new GeneralResponse<ProductDTO>
                {
                    Success = false,
                    Message = "Invalid Tech Company ID format. Expected GUID format.",
                    Data = null
                };
            }

            try
            {
                // Validate TechCompany exists and is active
                var techCompany = await _techCompanyRepo.GetFirstOrDefaultAsync(tc => tc.Id == dto.TechCompanyId);
                if (techCompany == null)
                {
                    return new GeneralResponse<ProductDTO>
                    {
                        Success = false,
                        Message = $"Tech Company with ID '{dto.TechCompanyId}' not found.",
                        Data = null
                    };
                }

                if (!techCompany.IsActive)
                {
                    return new GeneralResponse<ProductDTO>
                    {
                        Success = false,
                        Message = $"Tech Company '{techCompany.User?.UserName ?? dto.TechCompanyId}' is not active and cannot create products.",
                        Data = null
                    };
                }

                // Validate discount price
                if (dto.DiscountPrice.HasValue && dto.DiscountPrice >= dto.Price)
                {
                    return new GeneralResponse<ProductDTO>
                    {
                        Success = false,
                        Message = "Discount price must be less than the regular price.",
                        Data = null
                    };
                }

                // Find the category by enum value
                var categoryName = category.GetStringValue();
                var categoryEntity = await _categoryRepo.GetFirstOrDefaultAsync(c => c.Name == categoryName);
                if (categoryEntity == null)
                {
                    return new GeneralResponse<ProductDTO>
                    {
                        Success = false,
                        Message = $"Category '{categoryName}' not found.",
                        Data = null
                    };
                }

                // Find subcategory if provided
                SubCategory? subCategoryEntity = null;
                if (!string.IsNullOrWhiteSpace(dto.SubCategoryName))
                {
                    subCategoryEntity = await _subCategoryRepo.GetFirstOrDefaultAsync(sc => sc.Name == dto.SubCategoryName);
                    if (subCategoryEntity == null)
                    {
                        return new GeneralResponse<ProductDTO>
                        {
                            Success = false,
                            Message = $"SubCategory '{dto.SubCategoryName}' not found.",
                            Data = null
                        };
                    }
                }

                // Validate TechCompany exists and is active (already validated above, but double-check)
                if (techCompany == null)
                {
                    return new GeneralResponse<ProductDTO>
                    {
                        Success = false,
                        Message = $"Tech Company with ID '{dto.TechCompanyId}' not found.",
                        Data = null
                    };
                }

                var product = new Product
                {
                    Name = dto.Name,
                    Price = dto.Price,
                    DiscountPrice = dto.DiscountPrice,
                    Description = dto.Description,
                    Stock = dto.Stock,
                    CategoryId = categoryEntity.Id,
                    SubCategoryId = subCategoryEntity?.Id,
                    TechCompanyId = dto.TechCompanyId,
                    status = status,
                    Image1Url = dto.Image1Url,
                    Image2Url = dto.Image2Url,
                    Image3Url = dto.Image3Url,
                    Image4Url = dto.Image4Url
                };

                await _productRepo.AddAsync(product);
                await _productRepo.SaveChangesAsync();

                // Get the created product with all necessary includes for DTO mapping
                var addedProduct = await _productRepo.GetByIdWithIncludesAsync(
                    product.Id,
                    p => p.Category,
                    p => p.SubCategory,
                    p => p.TechCompany,
                    p => p.TechCompany.User,
                    p => p.Specifications,
                    p => p.Warranties);

                // Add specifications if provided
                if (dto.Specifications != null && dto.Specifications.Any())
                {
                    foreach (var specDto in dto.Specifications)
                    {
                        if (string.IsNullOrWhiteSpace(specDto.Key) || string.IsNullOrWhiteSpace(specDto.Value))
                        {
                            return new GeneralResponse<ProductDTO>
                            {
                                Success = false,
                                Message = "Specification key and value cannot be null or empty.",
                                Data = null
                            };
                        }

                        var specification = new Specification
                        {
                            Key = specDto.Key.Trim(),
                            Value = specDto.Value.Trim(),
                            ProductId = product.Id
                        };
                        await _specRepo.AddAsync(specification);
                    }
                }

                // Add warranties if provided
                if (dto.Warranties != null && dto.Warranties.Any())
                {
                    foreach (var warrantyDto in dto.Warranties)
                    {
                        if (string.IsNullOrWhiteSpace(warrantyDto.Type) || string.IsNullOrWhiteSpace(warrantyDto.Duration))
                        {
                            return new GeneralResponse<ProductDTO>
                            {
                                Success = false,
                                Message = "Warranty type and duration are required.",
                                Data = null
                            };
                        }

                        if (warrantyDto.StartDate >= warrantyDto.EndDate)
                        {
                            return new GeneralResponse<ProductDTO>
                            {
                                Success = false,
                                Message = "Warranty start date must be before end date.",
                                Data = null
                            };
                        }

                        var warranty = new Warranty
                        {
                            Type = warrantyDto.Type.Trim(),
                            Duration = warrantyDto.Duration,
                            Description = warrantyDto.Description?.Trim(),
                            StartDate = warrantyDto.StartDate,
                            EndDate = warrantyDto.EndDate,
                            ProductId = product.Id
                        };
                        await _warrantyRepo.AddAsync(warranty);
                    }
                }

                await _productRepo.SaveChangesAsync();

                // Get the final product with all specifications and warranties
                var finalProduct = await _productRepo.GetByIdWithIncludesAsync(
                    product.Id,
                    p => p.Category,
                    p => p.SubCategory,
                    p => p.TechCompany,
                    p => p.TechCompany.User,
                    p => p.Specifications,
                    p => p.Warranties);

                // Send notification to admin about new pending product
                await _notificationService.SendNotificationToRoleAsync(
                    "Admin",
                    $"New product '{product.Name}' has been added and is pending approval.",
                    NotificationType.ProductPending,
                    product.Id,
                    "Product"
                );

                return new GeneralResponse<ProductDTO>
                {
                    Success = true,
                    Message = "Product created successfully and is pending approval.",
                    Data = ProductMapper.MapToProductDTO(finalProduct)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<ProductDTO>
                {
                    Success = false,
                    Message = $"An unexpected error occurred while creating the product {ex}.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<ProductDTO>> UpdateAsync(string id, ProductUpdateDTO dto, ProductCategory category, ProductPendingStatus status)
        {
            
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<ProductDTO>
                {
                    Success = false,
                    Message = "Product ID cannot be null or empty.",
                    Data = null
                };
            }

            if (!Guid.TryParse(id, out _))
            {
                return new GeneralResponse<ProductDTO>
                {
                    Success = false,
                    Message = "Invalid Product ID format. Expected GUID format.",
                    Data = null
                };
            }

            if (dto == null)
            {
                return new GeneralResponse<ProductDTO>
                {
                    Success = false,
                    Message = "Product update data cannot be null.",
                    Data = null
                };
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return new GeneralResponse<ProductDTO>
                {
                    Success = false,
                    Message = "Product name is required.",
                    Data = null
                };
            }

            if (dto.Price <= 0)
            {
                return new GeneralResponse<ProductDTO>
                {
                    Success = false,
                    Message = "Product price must be greater than 0.",
                    Data = null
                };
            }

            if (dto.Stock < 0)
            {
                return new GeneralResponse<ProductDTO>
                {
                    Success = false,
                    Message = "Product stock cannot be negative.",
                    Data = null
                };
            }

            try
            {
                var product = await _productRepo.GetByIdWithIncludesAsync(
                    id,
                    p => p.Category,
                    p => p.SubCategory,
                    p => p.TechCompany,
                    p => p.Specifications,
                    p => p.Warranties);

                if (product == null)
                {
                    return new GeneralResponse<ProductDTO>
                    {
                        Success = false,
                        Message = $"Product with ID '{id}' not found.",
                        Data = null
                    };
                }

                // Validate TechCompany is still active
                if (product.TechCompany != null && !product.TechCompany.IsActive)
                {
                    return new GeneralResponse<ProductDTO>
                    {
                        Success = false,
                        Message = $"Cannot update product. Tech Company '{product.TechCompany.User?.UserName ?? product.TechCompanyId}' is not active.",
                        Data = null
                    };
                }

                // Validate discount price
                if (dto.DiscountPrice.HasValue && dto.DiscountPrice >= dto.Price)
                {
                    return new GeneralResponse<ProductDTO>
                    {
                        Success = false,
                        Message = "Discount price must be less than the regular price.",
                        Data = null
                    };
                }

                // Update basic properties
                product.Name = dto.Name;
                product.Price = dto.Price;
                product.DiscountPrice = dto.DiscountPrice;
                product.Description = dto.Description;
                product.Stock = dto.Stock;
                product.status = status;

                // Update category if changed
                if (category != ProductCategory.None)
                {
                    var categoryName = category.GetStringValue();
                    var categoryEntity = await _categoryRepo.GetFirstOrDefaultAsync(c => c.Name == categoryName);
                    if (categoryEntity != null)
                    {
                        product.CategoryId = categoryEntity.Id;
                    }
                }

                // Update subcategory if provided
                if (!string.IsNullOrWhiteSpace(dto.SubCategoryName))
                {
                    var subCategoryEntity = await _subCategoryRepo.GetFirstOrDefaultAsync(sc => sc.Name == dto.SubCategoryName);
                    if (subCategoryEntity != null)
                    {
                        product.SubCategoryId = subCategoryEntity.Id;
                    }
                }

                // Update specifications if provided
                if (dto.Specifications != null)
                {
                    // Validate specifications
                    foreach (var specDto in dto.Specifications)
                    {
                        if (string.IsNullOrWhiteSpace(specDto.Key) || string.IsNullOrWhiteSpace(specDto.Value))
                        {
                            return new GeneralResponse<ProductDTO>
                            {
                                Success = false,
                                Message = "Specification key and value cannot be null or empty.",
                                Data = null
                            };
                        }
                    }

                    // Remove existing specifications
                    var existingSpecs = await _specRepo.FindAsync(s => s.ProductId == id);
                    foreach (var spec in existingSpecs)
                    {
                        _specRepo.Remove(spec);
                    }

                    // Add new specifications
                    foreach (var specDto in dto.Specifications)
                    {
                        var specification = new Specification
                        {
                            Key = specDto.Key.Trim(),
                            Value = specDto.Value.Trim(),
                            ProductId = id
                        };
                        await _specRepo.AddAsync(specification);
                    }
                }

                // Update warranties if provided
                if (dto.Warranties != null)
                {
                    // Validate warranties
                    foreach (var warrantyDto in dto.Warranties)
                    {
                        if (string.IsNullOrWhiteSpace(warrantyDto.Type) || string.IsNullOrWhiteSpace(warrantyDto.Duration))
                        {
                            return new GeneralResponse<ProductDTO>
                            {
                                Success = false,
                                Message = "Warranty type and duration are required.",
                                Data = null
                            };
                        }

                        if (warrantyDto.StartDate >= warrantyDto.EndDate)
                        {
                            return new GeneralResponse<ProductDTO>
                            {
                                Success = false,
                                Message = "Warranty start date must be before end date.",
                                Data = null
                            };
                        }
                    }

                    // Remove existing warranties
                    var existingWarranties = await _warrantyRepo.FindAsync(w => w.ProductId == id);
                    foreach (var warranty in existingWarranties)
                    {
                        _warrantyRepo.Remove(warranty);
                    }

                    // Add new warranties
                    foreach (var warrantyDto in dto.Warranties)
                    {
                        var warranty = new Warranty
                        {
                            Type = warrantyDto.Type.Trim(),
                            Duration = warrantyDto.Duration,
                            Description = warrantyDto.Description?.Trim(),
                            StartDate = warrantyDto.StartDate,
                            EndDate = warrantyDto.EndDate,
                            ProductId = id
                        };
                        await _warrantyRepo.AddAsync(warranty);
                    }
                }

                _productRepo.Update(product);
                await _productRepo.SaveChangesAsync();

                // Get the updated product with all includes for DTO mapping
                var updatedProduct = await _productRepo.GetByIdWithIncludesAsync(
                    id,
                    p => p.Category,
                    p => p.SubCategory,
                    p => p.TechCompany,
                    p => p.TechCompany.User,
                    p => p.Specifications,
                    p => p.Warranties);

                return new GeneralResponse<ProductDTO>
                {
                    Success = true,
                    Message = "Product updated successfully.",
                    Data = ProductMapper.MapToProductDTO(updatedProduct)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<ProductDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while updating the product.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<bool>> DeleteAsync(string id)
        {
            
            if (string.IsNullOrWhiteSpace(id))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Product ID cannot be null or empty.",
                    Data = false
                };
            }

            if (!Guid.TryParse(id, out _))
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
                var product = await _productRepo.GetByIdAsync(id);
                if (product == null)
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Product not found.",
                        Data = false
                    };
                }

                _productRepo.Remove(product);
                await _productRepo.SaveChangesAsync();

                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = "Product deleted successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "An unexpected error occurred while deleting the product.",
                    Data = false
                };
            }
        }

        public async Task<GeneralResponse<bool>> ApproveProductAsync(string productId)
        {
            
            if (string.IsNullOrWhiteSpace(productId))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Product ID cannot be null or empty.",
                    Data = false
                };
            }

            if (!Guid.TryParse(productId, out _))
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
                var product = await _productRepo.GetByIdAsync(productId);
                if (product == null)
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Product not found.",
                        Data = false
                    };
                }

                if (product.status == ProductPendingStatus.Approved)
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Product is already approved.",
                        Data = false
                    };
                }

                product.status = ProductPendingStatus.Approved;
                _productRepo.Update(product);
                await _productRepo.SaveChangesAsync();

                // Send notification to TechCompany about product approval
                await _notificationService.SendNotificationAsync(
                    product.TechCompanyId,
                    $"Your product '{product.Name}' has been approved by admin",
                    NotificationType.ProductApproved,
                    product.Id,
                    "Product"
                );

                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = "Product approved successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "An unexpected error occurred while approving the product.",
                    Data = false
                };
            }
        }

        public async Task<GeneralResponse<bool>> RejectProductAsync(string productId, string reason)
        {
            
            if (string.IsNullOrWhiteSpace(productId))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Product ID cannot be null or empty.",
                    Data = false
                };
            }

            if (!Guid.TryParse(productId, out _))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Invalid Product ID format. Expected GUID format.",
                    Data = false
                };
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Rejection reason cannot be null or empty.",
                    Data = false
                };
            }

            try
            {
                var product = await _productRepo.GetByIdAsync(productId);
                if (product == null)
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Product not found.",
                        Data = false
                    };
                }

                if (product.status == ProductPendingStatus.Rejected)
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Product is already rejected.",
                        Data = false
                    };
                }

                product.status = ProductPendingStatus.Rejected;
                _productRepo.Update(product);
                await _productRepo.SaveChangesAsync();

                // Send notification to TechCompany about product rejection
                await _notificationService.SendNotificationAsync(
                    product.TechCompanyId,
                    $"Your product '{product.Name}' has been rejected. Reason: {reason}",
                    NotificationType.ProductRejected,
                    product.Id,
                    "Product"
                );

                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = $"Product rejected successfully. Reason: {reason}",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "An unexpected error occurred while rejecting the product.",
                    Data = false
                };
            }
        }

        public async Task<GeneralResponse<PaginatedDTO<ProductDTO>>> GetPendingProductsAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? sortBy = null,
            bool sortDescending = false)
        {
            
            if (pageNumber < 1)
            {
                return new GeneralResponse<PaginatedDTO<ProductDTO>>
                {
                    Success = false,
                    Message = "Page number must be greater than 0.",
                    Data = null
                };
            }

            if (pageSize < 1 || pageSize > 100)
            {
                return new GeneralResponse<PaginatedDTO<ProductDTO>>
                {
                    Success = false,
                    Message = "Page size must be between 1 and 100.",
                    Data = null
                };
            }

            try
            {
                var pendingProducts = await _productRepo.FindWithIncludesAsync(
                    p => p.status == ProductPendingStatus.Pending,
                    p => p.Category,
                    p => p.SubCategory,
                    p => p.TechCompany,
                    p => p.TechCompany.User,
                    p => p.Specifications,
                    p => p.Warranties);

                
                var sortedProducts = sortBy?.ToLower() switch
                {
                    "price" => sortDescending ? pendingProducts.OrderByDescending(p => p.Price) : pendingProducts.OrderBy(p => p.Price),
                    "name" => sortDescending ? pendingProducts.OrderByDescending(p => p.Name) : pendingProducts.OrderBy(p => p.Name),
                    "stock" => sortDescending ? pendingProducts.OrderByDescending(p => p.Stock) : pendingProducts.OrderBy(p => p.Stock),
                    _ => pendingProducts.OrderBy(p => p.Name) 
                };

                int totalItems = sortedProducts.Count();

                var items = sortedProducts
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(ProductMapper.MapToProductDTO)
                    .Where(dto => dto != null)
                    .ToList();

                var result = new PaginatedDTO<ProductDTO>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    Items = items
                };

                return new GeneralResponse<PaginatedDTO<ProductDTO>>
                {
                    Success = true,
                    Message = "Pending products retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<PaginatedDTO<ProductDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving pending products.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<PaginatedDTO<ProductCardDTO>>> GetProductsByCategoryAsync(
            ProductCategory categoryEnum,
            int pageNumber = 1,
            int pageSize = 10,
            string? sortBy = null,
            bool sortDescending = false)
        {
            
            if (pageNumber < 1)
            {
                return new GeneralResponse<PaginatedDTO<ProductCardDTO>>
                {
                    Success = false,
                    Message = "Page number must be greater than 0.",
                    Data = null
                };
            }

            if (pageSize < 1 || pageSize > 100)
            {
                return new GeneralResponse<PaginatedDTO<ProductCardDTO>>
                {
                    Success = false,
                    Message = "Page size must be between 1 and 100.",
                    Data = null
                };
            }

            try
            {
                var categoryName = categoryEnum.GetStringValue();
                var products = await _productRepo.FindWithIncludesAsync(
                    p => p.Category != null && p.Category.Name == categoryName && p.status == ProductPendingStatus.Approved,
                    p => p.Category,
                    p => p.SubCategory,
                    p => p.Specifications,
                    p => p.Warranties);

                
                var sortedProducts = sortBy?.ToLower() switch
                {
                    "price" => sortDescending ? products.OrderByDescending(p => p.Price) : products.OrderBy(p => p.Price),
                    "name" => sortDescending ? products.OrderByDescending(p => p.Name) : products.OrderBy(p => p.Name),
                    "stock" => sortDescending ? products.OrderByDescending(p => p.Stock) : products.OrderBy(p => p.Stock),
                    _ => products.OrderBy(p => p.Name) 
                };

                int totalItems = sortedProducts.Count();

                var items = sortedProducts
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(ProductMapper.MapToProductCard)
                    .ToList();

                var result = new PaginatedDTO<ProductCardDTO>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    Items = items
                };

                return new GeneralResponse<PaginatedDTO<ProductCardDTO>>
                {
                    Success = true,
                    Message = "Products retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<PaginatedDTO<ProductCardDTO>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving products by category.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<ImageUploadResponseDTO>> UploadProductImageAsync(IFormFile imageFile, string productId)
        {
            try
            {
                if (imageFile == null || imageFile.Length == 0)
                {
                    return new GeneralResponse<ImageUploadResponseDTO>
                    {
                        Success = false,
                        Message = "No image file provided",
                        Data = null
                    };
                }

                if (!_fileService.IsValidImageFile(imageFile))
                {
                    return new GeneralResponse<ImageUploadResponseDTO>
                    {
                        Success = false,
                        Message = "Invalid image file. Please upload a valid image (jpg, jpeg, png, gif, bmp, webp) with size less than 5MB",
                        Data = null
                    };
                }

                
                var product = await _productRepo.GetByIdAsync(productId);
                if (product == null)
                {
                    return new GeneralResponse<ImageUploadResponseDTO>
                    {
                        Success = false,
                        Message = "Product not found",
                        Data = null
                    };
                }

                
                var imagePath = await _fileService.UploadImageAsync(imageFile, "products");
                var imageUrl = _fileService.GetImageUrl(imagePath);

                
                product.ImageUrl = imagePath;
                _productRepo.Update(product);
                await _productRepo.SaveChangesAsync();

                return new GeneralResponse<ImageUploadResponseDTO>
                {
                    Success = true,
                    Message = "Product image uploaded successfully",
                    Data = new ImageUploadResponseDTO
                    {
                        Success = true,
                        Message = "Image uploaded successfully",
                        ImagePath = imagePath,
                        ImageUrl = imageUrl
                    }
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<ImageUploadResponseDTO>
                {
                    Success = false,
                    Message = $"Error uploading product image: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<bool>> DeleteProductImageAsync(string productId)
        {
            try
            {
                var product = await _productRepo.GetByIdAsync(productId);
                if (product == null)
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Product not found",
                        Data = false
                    };
                }

                if (string.IsNullOrEmpty(product.ImageUrl))
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Product has no image to delete",
                        Data = false
                    };
                }

                
                var deleted = await _fileService.DeleteImageAsync(product.ImageUrl);
                if (deleted)
                {
                    
                    product.ImageUrl = null;
                    _productRepo.Update(product);
                    await _productRepo.SaveChangesAsync();
                }

                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = "Product image deleted successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = $"Error deleting product image: {ex.Message}",
                    Data = false
                };
            }
        }
    }
}
