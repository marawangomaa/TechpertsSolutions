using Core.DTOs.Product;
using Core.Enums;
using Core.Entities;
using Core.Interfaces;
using TechpertsSolutions.Core.Entities;

namespace Service
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepo;

        public ProductService(IRepository<Product> productRepo)
        {
            _productRepo = productRepo;
        }

        public async Task<PaginatedDTO<ProductCardDTO>> GetAllAsync(
            int pageNumber = 1,
            int pageSize = 10,
            ProductPendingStatus? status = null,
            string? categoryId = null,
            string? nameSearch = null,
            string? sortBy = null,
            bool sortDescending = false)
        {
            var allProducts = (await _productRepo.GetAllAsync()).AsQueryable();

            // Apply filters
            if (status.HasValue)
                allProducts = allProducts.Where(p => p.status == status.Value);

            if (!string.IsNullOrWhiteSpace(categoryId))
                allProducts = allProducts.Where(p => p.CategoryId == categoryId);

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
                .Select(MapToProductCardDTO)
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
            var product = await _productRepo.GetByIdAsync(id);
            return product == null ? null : MapToProductDTO(product);
        }

        public async Task<ProductDTO> AddAsync(ProductCreateDTO dto)
        {
            var product = MapToProduct(dto);
            await _productRepo.AddAsync(product);
            await _productRepo.SaveChanges();
            return MapToProductDTO(product);
        }

        public async Task<bool> UpdateAsync(string id, ProductUpdateDTO dto)
        {
            var product = await _productRepo.GetByIdAsync(id);
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

            _productRepo.Update(product);
            await _productRepo.SaveChanges();
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return false;

            _productRepo.Remove(product);
            await _productRepo.SaveChanges();
            return true;
        }

        private ProductDTO MapToProductDTO(Product p)
        {
            return new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                Stock = p.Stock,
                CategoryId = p.CategoryId,
                SubCategoryId = p.SubCategoryId,
                TechManagerId = p.TechManagerId,
                StockControlManagerId = p.StockControlManagerId,
                ImageUrl = p.ImageUrl,
                Status = p.status,
                DiscountPrice = p.DiscountPrice,
                Specifications = p.Specifications?.Select(s => new SpecificationDTO
                {
                    Id = s.Id,
                    Key = s.Key,
                    Value = s.Value
                }).ToList(),
                Warranties = p.Warranties?.Select(w => new WarrantyDTO
                {
                    Id = w.Id,
                    Description = w.Description,
                    StartDate = w.StartDate,
                    EndDate = w.EndDate
                }).ToList()
            };
        }

        private Product MapToProduct(ProductCreateDTO dto)
        {
            return new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                Description = dto.Description,
                Stock = dto.Stock,
                CategoryId = dto.CategoryId,
                SubCategoryId = dto.SubCategoryId,
                TechManagerId = dto.TechManagerId,
                StockControlManagerId = dto.StockControlManagerId,
                ImageUrl = dto.ImageUrl,
                status = dto.Status,
                DiscountPrice = dto.DiscountPrice,
                Specifications = dto.Specifications?.Select(s => new Specification
                {
                    Key = s.Key,
                    Value = s.Value
                }).ToList(),
                Warranties = dto.Warranties?.Select(w => new Warranty
                {
                    Description = w.Description,
                    StartDate = w.StartDate,
                    EndDate = w.EndDate
                }).ToList()
            };
        }

        private ProductCardDTO MapToProductCardDTO(Product p)
        {
            return new ProductCardDTO
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                SubCategoryId = p.SubCategoryId,
                SubCategoryName = p.SubCategory?.Name,
                DiscountPrice = p.DiscountPrice,
                Status = p.status.ToString()
            };
        }
    }
}
