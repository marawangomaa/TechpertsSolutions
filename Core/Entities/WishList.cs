using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace TechpertsSolutions.Core.Entities
{
    public class WishList : BaseEntity
    {
        public string CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public List<WishListItem>? WishListItems { get; set; } = new List<WishListItem>();
    }
}
