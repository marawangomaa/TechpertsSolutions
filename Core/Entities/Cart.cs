using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace TechpertsSolutions.Core.Entities
{
    public class Cart : BaseEntity
    {
        public string CustomerId { get; set; }
        public Customer Customer { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<CartItem>? CartItems { get; set; } = new List<CartItem>(); 
        public List<PCAssemblyItem>? PCAssemblyItems { get; set; } = new List<PCAssemblyItem>(); 
        public Order? Order { get; set; }
    }
}
