namespace Core.DTOs.Cart
{
    public class PartialCheckoutDTO
    {
        public string CustomerId { get; set; }
        public List<string> ProductIds { get; set; } = new();
        public string? PromoCode { get; set; }
    }
} 