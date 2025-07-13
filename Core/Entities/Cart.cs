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
        public string Id { get; set; } = new Guid().ToString();
        public string CustomerId { get; set; }
        public Customer Customer { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<CartItem>? CartItems { get; set; }
        public ICollection<WishListItem>? WishListItems { get; set; }
        public ICollection<PCAssemblyItem>? PCAssemblyItems { get; set; }
        public Order? Order { get; set; }
    }
}
