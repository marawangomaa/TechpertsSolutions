using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace TechpertsSolutions.Core.Entities
{
    public class ServiceUsage : BaseEntity
    {
        public ServiceType ServiceType { get; set; }
        public decimal ServiceFees { get; set; }
        public DateTime UsedOn { get; set; }
        public int CallCount { get; set; }
        public string? MaintenanceId { get; set; }
        public Maintenance? Maintenance { get; set; }
        public List<Order>? Orders { get; set; } = new List<Order>();
        public List<PCAssembly>? PCAssemblies { get; set; } = new List<PCAssembly>();
    }
}
