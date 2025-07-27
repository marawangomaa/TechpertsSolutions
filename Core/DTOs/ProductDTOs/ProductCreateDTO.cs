using Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.ProductDTOs
{
    public class ProductCreateDTO
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Product name must be between 2 and 100 characters")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Product price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Product price must be greater than 0")]
        public decimal Price { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Product stock is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Product stock cannot be negative")]
        public int Stock { get; set; }

        public string? ImageUrl { get; set; }

        public string? SubCategoryName { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Discount price cannot be negative")]
        public decimal? DiscountPrice { get; set; }

        //[Required(ErrorMessage = "Tech Company ID is required")]
        public string TechCompanyId { get; set; } = null!;
        public List<SpecificationDTO>? Specifications { get; set; }
        public List<WarrantyDTO>? Warranties { get; set; }
    }
}
