using Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Product
{
    public class ProductUpdateDTO
    {
        public string Id { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Required]
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }
        
        public string? ImageUrl { get; set; }
        
        [Required]
        public ProductPendingStatus Status { get; set; }
        
        [Required]
        public string CategoryId { get; set; } = string.Empty;
        
        public string? SubCategoryId { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? DiscountPrice { get; set; }
        
        [Required]
        public string TechManagerId { get; set; } = string.Empty;
        
        [Required]
        public string StockControlManagerId { get; set; } = string.Empty;
        
        public List<SpecificationDTO>? Specifications { get; set; }
        public List<WarrantyDTO>? Warranties { get; set; }
    }
}
