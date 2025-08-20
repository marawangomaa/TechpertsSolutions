using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.MaintenanceDTOs
{
    public class MaintenanceDTO
    {
        public string Id { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public string TechCompanyName { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public string? Notes { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string? DeviceType { get; set; }
        public string? Issue { get; set; }
        public string? Priority { get; set; }
        public decimal? ServiceFee { get; set; }
        public List<string>? DeviceImages { get; set; } = new List<string>();
        public ServiceType ServiceType { get; set; }
        public DateTime WarrantyStart { get; set; }
        public DateTime WarrantyEnd { get; set; }
    }
}
