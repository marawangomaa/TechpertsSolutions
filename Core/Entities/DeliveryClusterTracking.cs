using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.Entities
{
    public class DeliveryClusterTracking: BaseEntity
    {
        public string clusterId { get; set; }
        public string Location { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime? LastRetryTime { get; set; }
        public DeliveryClusterStatus Status { get; set; }
        public string Driver { get; set; }
    }
}
