using Core.DTOs.ProductDTOs;
using TechpertsSolutions.Core.DTOs;
using Core.Enums;
using Core.Entities;
using Core.Interfaces;
using TechpertsSolutions.Core.Entities;
using Core.Interfaces.Services;
using Service.Utilities;

namespace Service
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<Specification> _specRepo;
        private readonly IRepository<Warranty> _warrantyRepo;

        public ProductService(IRepository<Product> productRepo,
            IRepository<Specification> specRepo,
            IRepository<Warranty> warrantyRepo)
        {
            _productRepo = productRepo;
            _specRepo = specRepo;
            _warrantyRepo = warrantyRepo;
        }

        public async Task<GeneralResponse<PaginatedDTO<ProductCardDTO>>> GetAllAsync(
            int pageNumber = 1,
            int pageSize = 10,
            ProductPendingStatus? status = null,
            string? categoryId = null,
            string? subCategoryId = null,
            string? nameSearch = null,
            string? sortBy = null,
            bool sortDescending = false)
        {
            // Input validation
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

            // Validate categoryId if provided
            if (!string.IsNullOrWhiteSpace(categoryId) && !Guid.TryParse(categoryId, out _))
            {
                return new GeneralResponse<PaginatedDTO<ProductCardDTO>>
                {
                    Success = false,
                    Message = "Invalid Category ID format. Expected GUID format.",
                    Data = null
                };
            }

            // Validate subCategoryId if provided
            if (!string.IsNullOrWhiteSpace(subCategoryId) && !Guid.TryParse(subCategoryId, out _))
            {
                return new GeneralResponse<PaginatedDTO<ProductCardDTO>>
                {
                    Success = false,
                    Message = "Invalid SubCategory ID format. Expected GUID format.",
                    Data = null
                };
            }

            try
            {
                //var allProducts = (await _productRepo.GetAllAsync()).AsQueryable();
                var allProducts = await _productRepo.GetAllWithIncludesAsync(p=>p.Category, p => p.SubCategory, p => p.StockControlManager, p => p.TechManager);

                // Apply filters
                if (status.HasValue)
                    allProducts = allProducts.Where(p => p.status == status.Value);

                if (!string.IsNullOrWhiteSpace(categoryId))
                    allProducts = allProducts.Where(p => p.CategoryId == categoryId);

                if (!string.IsNullOrWhiteSpace(subCategoryId))
                    allProducts = allProducts.Where(p => p.SubCategoryId == subCategoryId);


                if (!string.IsNullOrWhiteSpace(nameSearch))
                    allProducts = allProducts.Where(p => p.Name.Contains(nameSearch, StringComparison.OrdinalIgnoreCase));

                // Sorting
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
            // Input validation
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
                    p => p.StockControlManager, 
                    p => p.TechManager,
                    p => p.StockControlManager.User,
                    p => p.TechManager.User,
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

        public async Task<GeneralResponse<ProductDTO>> AddAsync(ProductCreateDTO dto)
        {
            // Input validation
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

            if (!string.IsNullOrWhiteSpace(dto.CategoryId) && !Guid.TryParse(dto.CategoryId, out _))
            {
                return new GeneralResponse<ProductDTO>
                {
                    Success = false,
                    Message = "Invalid Category ID format. Expected GUID format.",
                    Data = null
                };
            }

            if (!string.IsNullOrWhiteSpace(dto.SubCategoryId) && !Guid.TryParse(dto.SubCategoryId, out _))
            {
                return new GeneralResponse<ProductDTO>
                {
                    Success = false,
                    Message = "Invalid SubCategory ID format. Expected GUID format.",
                    Data = null
                };
            }

            try
            {
                var product = ProductMapper.MapToProduct(dto);
                await _productRepo.AddAsync(product);
                await _productRepo.SaveChangesAsync();
                var addedProductWithIncludes = await _productRepo.GetByIdWithIncludesAsync(
                 product.Id, // Use the ID of the newly added product
                 p => p.Category,
                 p => p.SubCategory,
                 p => p.StockControlManager,
                 p => p.TechManager,
                 p => p.StockControlManager.User, // Include nested User for manager names
                 p => p.TechManager.User,         // Include nested User for manager names
                 p => p.Warranties,               // Include collections
                 p => p.Specifications            // Include collections
                );
                
                return new GeneralResponse<ProductDTO>
                {
                    Success = true,
                    Message = "Product created successfully.",
                    Data = ProductMapper.MapToProductDTO(product)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<ProductDTO>
                {
                    Success = false,
                    Message = "An unexpected error occurred while creating the product.",
                    Data = null
                };
            }
        }

        public async Task<GeneralResponse<bool>> UpdateAsync(string id, ProductUpdateDTO dto)
        {
            // Input validation
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

            if (dto == null)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Update data cannot be null.",
                    Data = false
                };
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Product name is required.",
                    Data = false
                };
            }

            if (dto.Price <= 0)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Product price must be greater than 0.",
                    Data = false
                };
            }

            if (dto.Stock < 0)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "Product stock cannot be negative.",
                    Data = false
                };
            }

            try
            {
                var product = await _productRepo.GetByIdWithIncludesAsync(
                 id,
                 p => p.Category,
                 p => p.SubCategory,
                 p => p.StockControlManager,
                 p => p.TechManager,
                 p => p.StockControlManager.User,
                 p => p.TechManager.User,
                 p => p.Warranties,      // Include for updating and response
                 p => p.Specifications   // Include for updating and response
                 );
                
                if (product == null)
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = $"Product with ID '{id}' not found.",
                        Data = false
                    };
                }

                product.Name = dto.Name;
                product.Price = dto.Price;
                product.DiscountPrice = dto.DiscountPrice;
                product.Description = dto.Description;
                product.Stock = dto.Stock;
                product.CategoryId = dto.CategoryId;
                product.SubCategoryId = dto.SubCategoryId;
                product.TechManagerId = dto.TechManagerId;
                product.StockControlManagerId = dto.StockControlManagerId;
                product.status = dto.Status;
                product.ImageUrl = dto.ImageUrl;

                if (dto.Specifications != null)
                {
                    var currentSpecifications = product.Specifications?.ToList() ?? new List<Specification>();
                    var updatedSpecifications = new List<Specification>();

                    foreach (var specDto in dto.Specifications)
                    {
                        var existingSpec = currentSpecifications.FirstOrDefault(s => !string.IsNullOrEmpty(specDto.Id) && s.Id == specDto.Id);
                        if (existingSpec != null)
                        {
                            // Update existing specification
                            existingSpec.Key = specDto.Key;
                            existingSpec.Value = specDto.Value;
                            updatedSpecifications.Add(existingSpec);
                        }
                        else
                        {
                            // Add new specification
                            updatedSpecifications.Add(new Specification
                            {
                                Key = specDto.Key,
                                Value = specDto.Value,
                                ProductId = product.Id // Ensure foreign key is set
                            });
                        }
                    }

                    // Remove specifications that are no longer in the DTO
                    var specsToRemove = currentSpecifications.Where(s => !updatedSpecifications.Any(us => us.Id == s.Id)).ToList();
                    foreach (var spec in specsToRemove)
                    {
                        _specRepo.Remove(spec); // Assuming IRepository can remove child entities directly
                    }

                    product.Specifications = updatedSpecifications; // Assign the updated list
                }
                else
                {
                    // If DTO has null specs, clear existing specifications
                    if (product.Specifications != null)
                    {
                        foreach (var spec in product.Specifications.ToList())
                        {
                            _specRepo.Remove(spec);
                        }
                        product.Specifications.Clear();
                    }
                }

                // FIX: Handle Warranties updates (similar logic to Specifications)
                if (dto.Warranties != null)
                {
                    var currentWarranties = product.Warranties?.ToList() ?? new List<Warranty>();
                    var updatedWarranties = new List<Warranty>();

                    foreach (var warrantyDto in dto.Warranties)
                    {
                        var existingWarranty = currentWarranties.FirstOrDefault(w => !string.IsNullOrEmpty(warrantyDto.Id) && w.Id == warrantyDto.Id);
                        if (existingWarranty != null)
                        {
                            // Update existing warranty
                            existingWarranty.Description = warrantyDto.Description;
                            existingWarranty.StartDate = warrantyDto.StartDate;
                            existingWarranty.EndDate = warrantyDto.EndDate;
                            updatedWarranties.Add(existingWarranty);
                        }
                        else
                        {
                            // Add new warranty
                            updatedWarranties.Add(new Warranty
                            {
                                Description = warrantyDto.Description,
                                StartDate = warrantyDto.StartDate,
                                EndDate = warrantyDto.EndDate,
                                ProductId = product.Id // Ensure foreign key is set
                            });
                        }
                    }

                    // Remove warranties that are no longer in the DTO
                    var warrantiesToRemove = currentWarranties.Where(w => !updatedWarranties.Any(uw => uw.Id == w.Id)).ToList();
                    foreach (var warranty in warrantiesToRemove)
                    {
                        _warrantyRepo.Remove(warranty);
                    }

                    product.Warranties = updatedWarranties; // Assign the updated list
                }
                else
                {
                    // If DTO has null warranties, clear existing warranties
                    if (product.Warranties != null)
                    {
                        foreach (var warranty in product.Warranties.ToList())
                        {
                            _warrantyRepo.Remove(warranty);
                        }
                        product.Warranties.Clear();
                    }
                }

                _productRepo.Update(product);
                await _specRepo.SaveChangesAsync();
                await _warrantyRepo.SaveChangesAsync();
                await _productRepo.SaveChangesAsync();
                
                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = "Product updated successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = "An unexpected error occurred while updating the product.",
                    Data = false
                };
            }
        }

        public async Task<GeneralResponse<bool>> DeleteAsync(string id)
        {
            // Input validation
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
                var product = await _productRepo.GetByIdWithIncludesAsync(id, 
                    p => p.Specifications, 
                    p => p.Warranties);
                
                if (product == null)
                {
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = $"Product with ID '{id}' not found.",
                        Data = false
                    };
                }

                // Remove related specifications
                if (product.Specifications != null)
                {
                    foreach (var spec in product.Specifications.ToList())
                    {
                        _specRepo.Remove(spec);
                    }
                }

                // Remove related warranties
                if (product.Warranties != null)
                {
                    foreach (var warranty in product.Warranties.ToList())
                    {
                        _warrantyRepo.Remove(warranty);
                    }
                }

                // Remove the product
                _productRepo.Remove(product);
                
                // Save changes
                await _specRepo.SaveChangesAsync();
                await _warrantyRepo.SaveChangesAsync();
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
    }
}
