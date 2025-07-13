using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.Entities
{
    public class WishList : BaseEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string CustomerId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Customer? Customer { get; set; }
        public ICollection<WishListItem>? WishListItems { get; set; }
    }
}
