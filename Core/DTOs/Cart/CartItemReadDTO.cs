using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.DTOs.Cart
{
    public class CartItemReadDTO
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; } // New: Price of the product
        public int Quantity { get; set; } // New: Quantity of the product in the cart
        public string ImageUrl { get; set; } // New: Image URL of the product
        public int StockQuantity { get; set; } // New: Current stock of the product (for validation)
        public decimal ItemTotal => Price * Quantity; // New: Total for this specific item

    }
}
