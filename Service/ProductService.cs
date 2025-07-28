using Core.DTOs.ProductDTOs;
using TechpertsSolutions.Core.DTOs;
using Core.Enums;
using Core.Entities;
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
        private readonly IFileService _fileService;

        public ProductService(IRepository<Product> productRepo,
            IRepository<Specification> specRepo,
            IRepository<Warranty> warrantyRepo,
            IRepository<Category> categoryRepo,
            IRepository<SubCategory> subCategoryRepo,
            IFileService fileService)
        {
            _productRepo = productRepo;
            _specRepo = specRepo;
            _warrantyRepo = warrantyRepo;
            _categoryRepo = categoryRepo;
            _subCategoryRepo = subCategoryRepo;
            _fileService = fileService;
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
                var allProducts = (await _productRepo.GetAllWithIncludesAsync(p => p.Category, p => p.SubCategory)).AsQueryable();

                
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
                    _ => allProducts.OrderBy(p => p.Id)
                };

                int totalItems = allProducts.Count();

                var items = allProducts
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(ProductMapper.MapToProductCardDTO)
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
                var product = await _productRepo.GetByIdWithIncludesAsync(id,
                    p => p.Category,
                    p => p.SubCategory,
                    p => p.Warranties,
                    p => p.Specifications);

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

            try
            {
                
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

                
                SubCategory? subCategory = null;
                if (!string.IsNullOrWhiteSpace(dto.SubCategoryName))
                {
                    subCategory = await _subCategoryRepo.GetFirstOrDefaultAsync(sc => sc.Name == dto.SubCategoryName && sc.CategoryId == categoryEntity.Id);
                    if (subCategory == null)
                    {
                        return new GeneralResponse<ProductDTO>
                        {
                            Success = false,
                            Message = $"SubCategory '{dto.SubCategoryName}' not found in category '{categoryName}'.",
                            Data = null
                        };
                    }
                }

                
                var product = ProductMapper.MapToProduct(dto);
                if (product == null)
                {
                    return new GeneralResponse<ProductDTO>
                    {
                        Success = false,
                        Message = "Failed to map product data.",
                        Data = null
                    };
                }

                
                product.status = status;
                product.CategoryId = categoryEntity.Id;
                if (subCategory != null)
                    product.SubCategoryId = subCategory.Id;

                await _productRepo.AddAsync(product);
                await _productRepo.SaveChangesAsync();

                var addedProduct = await _productRepo.GetByIdWithIncludesAsync(
                    product.Id, p => p.Category, p => p.SubCategory, p => p.Warranties, p => p.Specifications, p => p.TechCompany);

                return new GeneralResponse<ProductDTO>
                {
                    Success = true,
                    Message = "Product created successfully.",
                    Data = ProductMapper.MapToProductDTO(addedProduct)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<ProductDTO>
                {
                    Success = false,
                    Message = $"Unexpected error: {ex.Message}",
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
                    Message = "Update data cannot be null.",
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

            try
            {
                var product = await _productRepo.GetByIdWithIncludesAsync(
                    id, p => p.Category, p => p.SubCategory, p => p.Warranties, p => p.Specifications);

                if (product == null)
                {
                    return new GeneralResponse<ProductDTO>
                    {
                        Success = false,
                        Message = $"Product with ID '{id}' not found.",
                        Data = null
                    };
                }

                
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

                
                SubCategory? subCategory = null;
                if (!string.IsNullOrWhiteSpace(dto.SubCategoryName))
                {
                    subCategory = await _subCategoryRepo.GetFirstOrDefaultAsync(sc => sc.Name == dto.SubCategoryName && sc.CategoryId == categoryEntity.Id);
                    if (subCategory == null)
                    {
                        return new GeneralResponse<ProductDTO>
                        {
                            Success = false,
                            Message = $"SubCategory '{dto.SubCategoryName}' not found in category '{categoryName}'.",
                            Data = null
                        };
                    }
                }

                
                ProductMapper.MapToProduct(dto, product);
                product.status = status;
                product.CategoryId = categoryEntity.Id;
                product.SubCategoryId = subCategory?.Id;

                _productRepo.Update(product);

                
                await _productRepo.SaveChangesAsync();

                
                var updatedProduct = await _productRepo.GetByIdWithIncludesAsync(
                    product.Id, 
                    p => p.Category, 
                    p => p.SubCategory, 
                    p => p.Warranties, 
                    p => p.Specifications, 
                    p => p.TechCompany
                );

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
                    Message = $"Unexpected error: {ex.Message}",
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
                    p => p.SubCategory);

                
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
                    .Select(ProductMapper.MapToProductCardDTO)
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
