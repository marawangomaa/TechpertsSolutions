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
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; } 
        public int Quantity { get; set; } 
        public string ImageUrl { get; set; } 
        public int Stock { get; set; } 
        public decimal ItemTotal => Price * Quantity; 

    }
}
