using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Orders
{
    public class OrderReadDTO
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } // e.g., "Pending", "Completed"
        public List<OrderItemReadDTO> OrderItems { get; set; } = new List<OrderItemReadDTO>();

    }
}
