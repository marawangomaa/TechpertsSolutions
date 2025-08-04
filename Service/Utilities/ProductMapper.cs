using Core.DTOs.ProductDTOs;
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

            var subCategoryNames = new List<string>();

            // If product is assigned to multiple subcategories via SubCategory.CategorySubCategories
            if (product.SubCategory != null && product.SubCategory.CategorySubCategories != null)
            {
                subCategoryNames = product.SubCategory.CategorySubCategories
                    .Select(cs => cs.SubCategory?.Name)
                    .Where(name => !string.IsNullOrEmpty(name))
                    .Distinct()
                    .ToList();
            }
            // If product has direct SubCategory assignment
            else if (product.SubCategory != null)
            {
                subCategoryNames.Add(product.SubCategory.Name);
            }

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
                SubCategoryNames = subCategoryNames,
                CategoryEnum = categoryEnum,
                ImageUrl = product.ImageUrl,
                Image1Url = product.Image1Url,
                Image2Url = product.Image2Url,
                Image3Url = product.Image3Url,
                Image4Url = product.Image4Url,
                Status = product.status,
                DiscountPrice = product.DiscountPrice,
                TechCompanyId = product.TechCompanyId, 
                TechCompanyName = product.TechCompany?.User?.FullName ?? string.Empty,
                TechCompanyAddress = product.TechCompany?.User?.Address ?? string.Empty,
                Specifications = product.Specifications?.Select(s => new SpecificationDTO
                {
                    Id = s.Id,
                    Key = s.Key,
                    Value = s.Value
                }).ToList(),
                Warranties = product.Warranties?.Select(w => new WarrantyDTO
                {
                    Id = w.Id,
                    Type = w.Type,
                    Duration = w.Duration,
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
                ImageUrl = null, 
                DiscountPrice = dto.DiscountPrice,
                TechCompanyId = dto.TechCompanyId,
                Image1Url = dto.Image1Url,
                Image2Url = dto.Image2Url,
                Image3Url = dto.Image3Url,
                Image4Url = dto.Image4Url,
                Specifications = dto.Specifications?.Select(s => new Specification
                {
                    Key = s.Key,
                    Value = s.Value
                }).ToList(),
                Warranties = dto.Warranties?.Select(w => new Warranty
                {
                    Type = w.Type,
                    Duration = w.Duration,
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
            

            return existingProduct;
        }

        public static ProductCardDTO? MapToProductCardDTO(Product? product)
        {
            if (product == null) return null;

            
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

        public static ProductCardDTO? MapToProductCard(Product? product)
        {
            if (product == null) return null;

            return new ProductCardDTO
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                DiscountPrice = product.DiscountPrice,
                Status = product.status.ToString(),
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name ?? string.Empty,
                SubCategoryId = product.SubCategoryId,
                SubCategoryName = product.SubCategory?.Name ?? string.Empty,
                CategoryEnum = EnumExtensions.ParseFromStringValue<ProductCategory>(product.Category?.Name)
            };
        }
    }
} 
