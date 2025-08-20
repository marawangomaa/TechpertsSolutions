using Core.DTOs.MaintenanceDTOss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.MaintenanceDTOs
{
    public class MaintenanceDetailsDTO
    {
        public string Id { get; set; } = null!;
        public string? Notes { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string? DeviceType { get; set; }
        public string? Issue { get; set; }
        public string? Priority { get; set; }
        public decimal? ServiceFee { get; set; }
        public List<string>? DeviceImages { get; set; } = new List<string>();
        public MaintenanceCustomerDTO Customer { get; set; } = null!;
        public MaintenanceTechCompanyDTO TechCompany { get; set; } = null!;
        public MaintenanceWarrantyDTO Warranty { get; set; } = null!;
        public MaintenanceProductDTO Product { get; set; } = null!;
        public MaintenanceServiceUsageDTO ServiceUsage { get; set; } = null!;
    }
}
