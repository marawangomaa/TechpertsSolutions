using Core.Enums;

namespace Core.DTOs.MaintenanceDTOs
{
    public class MaintenanceCreateDTO
    {
        public string CustomerId { get; set; } = string.Empty;
        public string? TechCompanyId { get; set; }
        public string? WarrantyId { get; set; }
        public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Requested;
        public string? Notes { get; set; }
    }
}
