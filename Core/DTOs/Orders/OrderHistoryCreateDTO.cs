using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Orders
{
    public class OrderHistoryCreateDTO
    {
        public List<string> OrderIds { get; set; }
    }
}
