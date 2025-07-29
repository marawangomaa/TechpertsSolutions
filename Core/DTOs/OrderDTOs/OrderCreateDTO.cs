using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.OrderDTOs
{
    public class OrderCreateDTO
    {
        public string CustomerId { get; set; }
        public List<OrderItemCreateDTO> OrderItems { get; set; }
        public string DeliveryId { get; set; }
        public string ServiceUsageId { get; set; }
    }
}
