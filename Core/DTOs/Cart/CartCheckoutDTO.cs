using System;
using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.Cart
{
    public class CartCheckoutDTO
    {
        [Required]
        public string CustomerId { get; set; } = string.Empty;
        
        public string? DeliveryId { get; set; }
        
        public string? SalesManagerId { get; set; }
        
        public string? ServiceUsageId { get; set; }
    }
} 