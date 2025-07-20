using Core.DTOs.Product;
using Core.Entities;
using TechpertsSolutions.Core.Entities;
using System.Linq;

namespace Service.Utilities
{
    public static class ProductMapper
    {
        public static ProductDTO MapToProductDTO(Product product)
        {
            if (product == null) return null;

            return new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                SubCategoryId = product.SubCategoryId,
                TechManagerId = product.TechManagerId,
                StockControlManagerId = product.StockControlManagerId,
                CategoryName = product.Category?.Name,
                SubCategoryName = product.SubCategory?.Name,
                TechManagerName = product.TechManager?.User?.FullName,
                StockControlManagerName = product.StockControlManager?.User?.FullName,
                ImageUrl = product.ImageUrl,
                Status = product.status,
                DiscountPrice = product.DiscountPrice,
                Specifications = product.Specifications?.Select(s => new SpecificationDTO
                {
                    Id = s.Id,
                    Key = s.Key,
                    Value = s.Value
                }).ToList(),
                Warranties = product.Warranties?.Select(w => new WarrantyDTO
                {
                    Id = w.Id,
                    Description = w.Description,
                    StartDate = w.StartDate,
                    EndDate = w.EndDate
                }).ToList()
            };
        }

        public static Product MapToProduct(ProductCreateDTO dto)
        {
            if (dto == null) return null;

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

        public static ProductCardDTO MapToProductCardDTO(Product product)
        {
            if (product == null) return null;

            return new ProductCardDTO
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                SubCategoryId = product.SubCategoryId,
                SubCategoryName = product.SubCategory?.Name,
                DiscountPrice = product.DiscountPrice,
                Status = product.status.ToString()
            };
        }

        public static ProductListItemDTO MapToProductListItem(Product product)
        {
            if (product == null) return null;

            return new ProductListItemDTO
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl
            };
        }
    }
} 