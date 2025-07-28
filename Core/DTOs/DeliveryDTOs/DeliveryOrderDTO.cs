using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.DeliveryDTOs
{
    public class DeliveryOrderDTO
    {
        public string Id { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string CustomerName { get; set; } = null!;
        public string? City { get; set; }
        public string Status { get; set; } = null!;
    }
}
