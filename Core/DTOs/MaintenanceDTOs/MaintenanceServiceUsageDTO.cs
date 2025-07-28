using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.MaintenanceDTOs
{
    public class MaintenanceServiceUsageDTO
    {
        public string Id { get; set; } = null!;
        public string ServiceType { get; set; } = null!;
        public DateTime UsedOn { get; set; }
        public int CallCount { get; set; }
    }
}
