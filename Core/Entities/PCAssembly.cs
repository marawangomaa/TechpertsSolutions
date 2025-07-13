using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.Entities
{
    public class PCAssembly : BaseEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public string ServiceUsageId { get; set; }
        public ServiceUsage? ServiceUsage { get; set; }
        public ICollection<PCAssemblyItem>? PCAssemblyItems { get; set; }
    }
}
