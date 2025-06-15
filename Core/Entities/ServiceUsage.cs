using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.Entities
{
    public class ServiceUsage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CallCount { get; set; }
        public ICollection<Order>? Orders { get; set; }
        public ICollection<PCAssembly>? PCAssemblies { get; set; }

    }
}
