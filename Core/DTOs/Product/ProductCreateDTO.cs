using Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Product
{
    public class ProductCreateDTO
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
        public ProductPendingStatus Status { get; set; }
        public string CategoryId { get; set; }
        [Required]
        public ProductCategory Category { get; set; } // Enum instead of string
        public string? SubCategoryId { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string TechManagerId { get; set; }
        public string StockControlManagerId { get; set; }
        public List<SpecificationDTO>? Specifications { get; set; }
        public List<WarrantyDTO>? Warranties { get; set; }
    }
}
