using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Orders
{
    public class OrderHistoryReadDTO
    {
        public string Id { get; set; }
        public List<OrderReadDTO> Orders { get; set; }
    }
}
