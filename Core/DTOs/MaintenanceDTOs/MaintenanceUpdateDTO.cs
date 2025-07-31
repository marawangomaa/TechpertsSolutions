using Core.Enums;

namespace Core.DTOs.MaintenanceDTOs
{
    public class MaintenanceUpdateDTO
    {
        public string? CustomerId { get; set; }
        public string? TechCompanyId { get; set; }
        public string? WarrantyId { get; set; }
        public MaintenanceStatus? Status { get; set; }
        public string? Notes { get; set; }
        public DateTime? CompletedDate { get; set; }
    }
}
