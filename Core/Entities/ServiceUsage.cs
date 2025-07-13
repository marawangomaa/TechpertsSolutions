using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.Entities
{
    public class ServiceUsage : BaseEntity
    {
        public string Id { get; set; } = new Guid().ToString();
        public string ServiceType { get; set; }
        public DateTime UsedOn { get; set; }
        public int CallCount { get; set; }
        public ICollection<Order>? Orders { get; set; }
        public ICollection<PCAssembly>? PCAssemblies { get; set; }
    }
}
