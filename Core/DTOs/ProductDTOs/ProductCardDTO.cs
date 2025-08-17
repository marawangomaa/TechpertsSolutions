using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;
using Core.Utilities;

namespace Core.DTOs.ProductDTOs
{
    public class ProductCardDTO
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }  
        public string? ImageUrl { get; set; }
        public List<string>? ImageURLs { get; set; }
        public string? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public ProductCategory? CategoryEnum { get; set; }
        public string? SubCategoryId { get; set; }
        public string? SubCategoryName { get; set; }
        public List<SpecificationDTO>? Specifications { get; set; }
        public List<WarrantyDTO>? Warranties { get; set; }
        public string Status { get; set; } = "Pending";
        public string TechCompanyId { get; set; }
        public string? TechCompanyName { get; set; }
    }
}
