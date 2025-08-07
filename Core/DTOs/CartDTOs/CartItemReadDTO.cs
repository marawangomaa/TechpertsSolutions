using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.DTOs.CartDTOs
{
    public class CartItemReadDTO
    {
        public string Id { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; } 
        public int Quantity { get; set; } 
        public string ImageUrl { get; set; } = string.Empty;
        public int Stock { get; set; } 
        public decimal ItemTotal { get; set; }
        public bool IsCustomBuild { get; set; }
        public decimal? AssemblyFee { get; set; }
        public decimal? ProductTotal { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
