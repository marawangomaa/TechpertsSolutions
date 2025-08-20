using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.ServiceUsageDTOs
{
    public class ServiceUsageReadDTO
    {
        public string Id { get; set; }
        public ServiceType ServiceType { get; set; }
        public DateTime UsedOn { get; set; }
        public int CallCount { get; set; }
    }
}
