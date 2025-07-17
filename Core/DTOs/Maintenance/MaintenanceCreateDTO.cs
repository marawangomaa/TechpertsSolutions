using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Maintenance
{
    public class MaintenanceCreateDTO
    {
        public string CustomerId { get; set; } = null!;
        public string TechCompanyId { get; set; } = null!;
        public string WarrantyId { get; set; } = null!;
        public string ServiceUsageId { get; set; } = null!;
    }
}
