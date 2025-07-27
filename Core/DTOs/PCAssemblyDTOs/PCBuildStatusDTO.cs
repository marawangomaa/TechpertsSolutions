using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.PCAssemblyDTOs
{
    public class PCBuildStatusDTO
    {
        public string AssemblyId { get; set; }
        public string CustomerId { get; set; }
        public string? Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalPrice { get; set; }
        public List<PCBuildComponentDTO> Components { get; set; } = new List<PCBuildComponentDTO>();
        public Dictionary<string, bool> ComponentStatus { get; set; } = new Dictionary<string, bool>();
    }

    public class PCBuildComponentDTO
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
        public bool IsSelected { get; set; }
    }

    public class PCBuildTotalDTO
    {
        public string AssemblyId { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public int TotalComponents { get; set; }
        public int SelectedComponents { get; set; }
    }

    public class CompatibleComponentDTO
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string CompatibilityReason { get; set; }
        public double CompatibilityScore { get; set; }
    }
} 