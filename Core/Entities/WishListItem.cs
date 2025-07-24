using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace TechpertsSolutions.Core.Entities
{
    public class WishListItem : BaseEntity
    {
        public string ProductId { get; set; }
        public Product? Product { get; set; }
        public string CartId { get; set; }
        public Cart? Cart { get; set; }
        public string WishListId { get; set; }
        public WishList? WishList { get; set; }
    }
}
