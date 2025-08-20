using Core.Enums;

namespace Core.DTOs.MaintenanceDTOs
{
    public class MaintenanceCreateDTO
    {
        public string CustomerId { get; set; }
        public string? TechCompanyId { get; set; }
        public string? WarrantyId { get; set; }
        public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Requested;
        public string? Notes { get; set; }
        public string? DeviceType { get; set; }
        public string? Issue { get; set; }
        public string? Priority { get; set; }
        public List<string>? DeviceImages { get; set; } = new List<string>();
    }
}
