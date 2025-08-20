using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.ServiceUsageDTOs
{
    public class ServiceUsageUpdateDTOs
    {
        public ServiceType? ServiceType { get; set; }
        public decimal ServiceFees { get; set; }
        public DateTime? UsedOn { get; set; }
        public int? CallCount { get; set; }
    }
}
