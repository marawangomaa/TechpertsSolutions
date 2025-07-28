using Core.DTOs.ProductDTOs;
using Core.Entities;
using TechpertsSolutions.Core.Entities;
using System.Linq;
using Core.Utilities;
using Core.Enums;

namespace Service.Utilities
{
    public static class ProductMapper
    {
        public static ProductDTO? MapToProductDTO(Product? product)
        {
            if (product == null) return null;

            // Convert category name to enum
            ProductCategory? categoryEnum = null;
            try
            {
                if (!string.IsNullOrEmpty(product.Category?.Name))
                {
                    categoryEnum = EnumExtensions.ParseFromStringValue<ProductCategory>(product.Category.Name);
                }
            }
            catch
            {
                // If conversion fails, leave as null
            }

            return new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                SubCategoryId = product.SubCategoryId,
                CategoryName = product.Category?.Name ?? string.Empty,
                SubCategoryName = product.SubCategory?.Name,
                CategoryEnum = categoryEnum,
                ImageUrl = product.ImageUrl,
                Status = product.status,
                DiscountPrice = product.DiscountPrice,
                TechCompanyId = product.TechCompanyId, // Assuming same as TechManager for now
                TechCompanyName = product.TechCompany?.User?.UserName ?? string.Empty,
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

        public static Product? MapToProduct(ProductCreateDTO? dto)
        {
            if (dto == null) return null;

            return new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                Description = dto.Description,
                Stock = dto.Stock,
                ImageUrl = null, // Image will be handled separately via upload
                DiscountPrice = dto.DiscountPrice,
                TechCompanyId = dto.TechCompanyId, 
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

        public static Product? MapToProduct(ProductUpdateDTO? dto, Product? existingProduct)
        {
            if (dto == null || existingProduct == null) return null;

            existingProduct.Name = dto.Name;
            existingProduct.Price = dto.Price;
            existingProduct.DiscountPrice = dto.DiscountPrice;
            existingProduct.Description = dto.Description;
            existingProduct.Stock = dto.Stock;
            // ImageUrl is not updated here - use separate upload endpoint

            return existingProduct;
        }

        public static ProductCardDTO? MapToProductCardDTO(Product? product)
        {
            if (product == null) return null;

            // Convert category name to enum
            ProductCategory? categoryEnum = null;
            try
            {
                if (!string.IsNullOrEmpty(product.Category?.Name))
                {
                    categoryEnum = EnumExtensions.ParseFromStringValue<ProductCategory>(product.Category.Name);
                }
            }
            catch
            {
                // If conversion fails, leave as null
            }

            return new ProductCardDTO
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name ?? string.Empty,
                CategoryEnum = categoryEnum,
                SubCategoryId = product.SubCategoryId,
                SubCategoryName = product.SubCategory?.Name,
                DiscountPrice = product.DiscountPrice,
                Status = product.status.ToString()
            };
        }

        public static ProductListItemDTO? MapToProductListItem(Product? product)
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