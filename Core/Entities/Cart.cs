using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechpertsSolutions.Core.Entities
{
    public class Cart : BaseEntity
    {
        public string CustomerId { get; set; }
        public Customer Customer { get; set; }
        public List<CartItem>? CartItems { get; set; } = new List<CartItem>(); 
    }
}
