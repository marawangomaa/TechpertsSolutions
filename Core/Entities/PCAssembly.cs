using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.Entities
{
    public class PCAssembly
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public ICollection<PCAssemblyItem>? PCAssemblyItems { get; set; }
    }
}
