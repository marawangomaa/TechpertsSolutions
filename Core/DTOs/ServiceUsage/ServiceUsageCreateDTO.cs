using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.ServiceUsage
{
    public class ServiceUsageCreateDTO
    {
        public string ServiceType { get; set; }
        public DateTime UsedOn { get; set; }
        public int CallCount { get; set; }
    }
}
