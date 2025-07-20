using Core.DTOs.Product;
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

        public async Task<PaginatedDTO<ProductCardDTO>> GetAllAsync(
            int pageNumber = 1,
            int pageSize = 10,
            ProductPendingStatus? status = null,
            string? categoryId = null,
            string? subCategoryId = null,
            string? nameSearch = null,
            string? sortBy = null,
            bool sortDescending = false)
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

            return new PaginatedDTO<ProductCardDTO>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                Items = items
            };
        }

        public async Task<ProductDTO?> GetByIdAsync(string id)
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
            return product == null ? null : ProductMapper.MapToProductDTO(product);
        }

        public async Task<ProductDTO> AddAsync(ProductCreateDTO dto)
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
            return ProductMapper.MapToProductDTO(product);
        }

        public async Task<bool> UpdateAsync(string id, ProductUpdateDTO dto)
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
            if (product == null) return false;

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
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var product = await _productRepo.GetByIdWithIncludesAsync(id, 
                p => p.Specifications, 
                p => p.Warranties);
            
            if (product == null) return false;

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
            
            return true;
        }


    }
}
