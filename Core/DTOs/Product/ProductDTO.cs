using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Product
{
    public class ProductDTO
    {
        public string Id { get; set; }

        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public int Stock { get; set; }
        public ProductPendingStatus Status { get; set; }
        public string? CategoryId { get; set; }
        public string? SubCategoryId { get; set; }
        public string? TechManagerId { get; set; }
        public string? StockControlManagerId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? SubCategoryName { get; set; }
        public string? ImageUrl { get; set; }
        public string TechManagerName { get; set; } = null!;
        public string StockControlManagerName { get; set; } = null!;

        public List<SpecificationDTO>? Specifications { get; set; }
        public List<WarrantyDTO>? Warranties { get; set; }  
    }
}
