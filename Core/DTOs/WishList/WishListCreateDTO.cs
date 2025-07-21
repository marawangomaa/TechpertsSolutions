using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.WishList
{
    public class WishListCreateDTO
    {
        public string CustomerId { get; set; }
        public List<WishListItemCreateDTO>? Items { get; set; }
    }
}
