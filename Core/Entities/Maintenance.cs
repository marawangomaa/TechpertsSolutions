using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;
using Core.Enums;

namespace TechpertsSolutions.Core.Entities
{
    public class Maintenance : BaseEntity
    {
        public string? WarrantyId { get; set; }
        public Warranty? Warranty { get; set; }
        public string? TechCompanyId { get; set; }
        public TechCompany? TechCompany { get; set; }
        public string CustomerId { get; set; }
        public Customer? Customer{ get; set; }
        public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Requested;
        public string? Notes { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string? DeviceType { get; set; }
        public string? Issue { get; set; }
        public string? Priority { get; set; }
        public decimal? ServiceFee { get; set; }
        public List<string>? DeviceImages { get; set; } = new List<string>();
        public List<ServiceUsage>? ServiceUsages { get; set; } = new List<ServiceUsage>();
    }
}
