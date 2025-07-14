using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Cart
{
    public class CartUpdateItemQuantityDTO
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
