using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.Entities
{
    public class PCAssemblyItem
    {
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int CartId { get; set; }
        public Cart? Cart { get; set; }
        public int PCAssemblyId { get; set; }
        public PCAssembly? WishList { get; set; }
        public bool IsAssemblied { get; set; } = true;
    }
}
