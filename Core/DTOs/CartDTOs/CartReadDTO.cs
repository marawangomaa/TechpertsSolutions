using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.CartDTOs
{
    public class CartReadDTO
    {
        public string Id { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<CartItemReadDTO> CartItems { get; set; } = new List<CartItemReadDTO>();
        public decimal SubTotal { get; set; } 
    }
}
