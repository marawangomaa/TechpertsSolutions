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
        public string ServiceType { get; set; } = null!;
        public DateTime WarrantyStart { get; set; }
        public DateTime WarrantyEnd { get; set; }
    }
}
