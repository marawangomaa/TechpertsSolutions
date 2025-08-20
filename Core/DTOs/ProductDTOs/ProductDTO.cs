using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Utilities;

namespace Core.DTOs.ProductDTOs
{
    public class ProductDTO
    {
        public string Id { get; set; }

        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string? Description { get; set; }
        public int Stock { get; set; }
        public ProductPendingStatus Status { get; set; }
        public string? CategoryId { get; set; }
        public string? SubCategoryId { get; set; }
        public string? TechCompanyId { get; set; }
        public string CategoryName { get; set; } = null!;
        public ProductCategory? CategoryEnum { get; set; }
        public string? SubCategoryName { get; set; }        
        public string? ImageUrl { get; set; }
        public List<string>? ImageUrls { get; set; }

        public string? TechCompanyName { get; set; }
        public string TechCompanyAddress { get; set; } = null!;
        public string TechCompanyUserId { get; set; } = null!;
        public string TechCompanyImage { get; set; } = null!;
        public List<SpecificationDTO>? Specifications { get; set; }
        public List<WarrantyDTO>? Warranties { get; set; }  
    }
}
