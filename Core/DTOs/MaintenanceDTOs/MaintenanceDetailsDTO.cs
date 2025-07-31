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
        public MaintenanceCustomerDTO Customer { get; set; } = null!;
        public MaintenanceTechCompanyDTO TechCompany { get; set; } = null!;
        public MaintenanceWarrantyDTO Warranty { get; set; } = null!;
        public MaintenanceProductDTO Product { get; set; } = null!;
        public MaintenanceServiceUsageDTO ServiceUsage { get; set; } = null!;
    }
}
