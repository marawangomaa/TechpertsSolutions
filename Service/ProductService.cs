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

        public async Task<IEnumerable<ProductCardDTO>> GetAllAsync(
            ProductPendingStatus? status = null,
            string? categoryId = null,
            string? nameSearch = null,
            string? sortBy = null,
            bool sortDescending = false)
        {
            var products = await _productRepo.GetAllAsync();

            // Filters
            if (status.HasValue)
                products = products.Where(p => p.status == status.Value);

            if (!string.IsNullOrWhiteSpace(categoryId))
                products = products.Where(p => p.CategoryId == categoryId);

            if (!string.IsNullOrWhiteSpace(nameSearch))
                products = products.Where(p => p.Name.Contains(nameSearch, StringComparison.OrdinalIgnoreCase));

            // Sorting
            products = sortBy?.ToLower() switch
            {
                "price" => sortDescending ? products.OrderByDescending(p => p.Price) : products.OrderBy(p => p.Price),
                "name" => sortDescending ? products.OrderByDescending(p => p.Name) : products.OrderBy(p => p.Name),
                "stock" => sortDescending ? products.OrderByDescending(p => p.Stock) : products.OrderBy(p => p.Stock),
                _ => products.OrderBy(p => p.Id)
            };

            return products.Select(MapToProductCardDTO).ToList();
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

        // Mapping

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
                Status = p.status.ToString()
            };
        }
    }
}
