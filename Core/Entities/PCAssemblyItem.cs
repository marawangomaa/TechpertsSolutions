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
        public string Id { get; set; } = new Guid().ToString();
        public string ProductId { get; set; }
        public Product? Product { get; set; }
        public string CartId { get; set; }
        public Cart? Cart { get; set; }
        public string PCAssemblyId { get; set; }
        public PCAssembly? PCAssembly { get; set; }
        public bool IsAssemblied { get; set; } = true;
    }
}
