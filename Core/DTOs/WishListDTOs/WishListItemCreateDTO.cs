using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.WishListDTOs
{
    public class WishListItemCreateDTO
    {
        public string ProductId { get; set; }
        public string? CartId { get; set; }
    }
}
