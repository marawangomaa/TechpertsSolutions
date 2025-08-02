using Core.Enums;

namespace Core.DTOs.PCAssemblyDTOs
{
    public class PCBuildTableDTO
    {
        public string AssemblyId { get; set; } = string.Empty;
        public List<PCBuildTableItemDTO> Components { get; set; } = new List<PCBuildTableItemDTO>();
        public decimal TotalCost { get; set; }
        public decimal AssemblyFee { get; set; }
        public decimal GrandTotal { get; set; }
        public bool IsComplete { get; set; }
    }

    public class PCBuildTableItemDTO
    {
        public ProductCategory ComponentType { get; set; }
        public string ComponentDisplayName { get; set; } = string.Empty;
        public string? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? SubCategoryName { get; set; }
        public string Status { get; set; } = "Not Selected";
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }
        public bool HasComponent { get; set; }
        public string? ItemId { get; set; }
    }

    public class PCBuildTableResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public PCBuildTableDTO? Data { get; set; }
    }
} 