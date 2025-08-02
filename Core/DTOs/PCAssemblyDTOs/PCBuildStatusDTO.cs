using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.PCAssemblyDTOs
{
    public class PCBuildStatusDTO
    {
        public string AssemblyId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int ComponentCount { get; set; }
        public decimal TotalCost { get; set; }
        public bool IsComplete { get; set; }
    }

    public class PCBuildComponentDTO
    {
        public string Id { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string SubCategory { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
        public bool IsSelected { get; set; }
    }

    public class PCBuildTotalDTO
    {
        public string AssemblyId { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal AssemblyFee { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class CompatibleComponentDTO
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal CompatibilityScore { get; set; }
    }
} 
