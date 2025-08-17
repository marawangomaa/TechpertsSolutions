using Core.DTOs.ProductDTOs;
using Core.Enums;
using Core.Interfaces.Services;
using Core.Utilities;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using TechpertsSolutions.Core.Entities;

namespace Service.Utilities
{
    public static class ProductMapper
    {
        public static ProductDTO? MapToProductDTO(Product? product)
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
                SubCategoryName = product.SubCategory?.Name ?? string.Empty,
                CategoryEnum = categoryEnum,
                ImageUrl = product.ImageUrl ?? "assets/products/default-product.png",
                ImageUrls = (product.ImagesURLS != null && product.ImagesURLS.Any()) ? product.ImagesURLS : new List<string>
                                                                                      {
                                                                                        "assets/products/default-product.png",
                                                                                        "assets/products/default-product.png",
                                                                                        "assets/products/default-product.png",
                                                                                        "assets/products/default-product.png"
                                                                                      },
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

        public static ProductCardDTO? MapToProductCard(Product? product)
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
                DiscountPrice = product.DiscountPrice,
                Status = product.status.ToString(),
                ImageUrl = product.ImageUrl ?? "assets/products/default-product.png",
                ImageURLs = (product.ImagesURLS != null && product.ImagesURLS.Any())? product.ImagesURLS: new List<string>
                                                                                      {
                                                                                        "assets/products/default-product.png",
                                                                                        "assets/products/default-product.png",
                                                                                        "assets/products/default-product.png",
                                                                                        "assets/products/default-product.png"
                                                                                      },
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name ?? string.Empty,
                SubCategoryId = product.SubCategoryId,
                SubCategoryName = product.SubCategory?.Name ?? string.Empty,
                CategoryEnum = categoryEnum,
                TechCompanyId = product.TechCompanyId,
                TechCompanyName = product.TechCompany?.User?.FullName ?? string.Empty,
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
    }
} 
