using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace TechpertsSolutions.Core.Entities
{
    public class PCAssemblyItem : BaseEntity
    {
        public string ProductId { get; set; }
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public string PCAssemblyId { get; set; }
        public PCAssembly? PCAssembly { get; set; }
        public bool IsAssembled { get; set; } = true;
    }
}
