using Core.Enums;

namespace Core.DTOs.PCAssemblyDTOs
{
    public class PCAssemblyItemDTO
    {
        public string ItemId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string? SubCategoryName { get; set; }
        public ProductCategory Category { get; set; }
        public ProductPendingStatus Status { get; set; }
        public decimal Price { get; set; }
        public decimal? Discount { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }
} 