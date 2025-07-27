using Core.Entities;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechpertsSolutions.Core.Entities
{
    public class Order : BaseEntity
    {
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public string CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public string CartId { get; set; }
        public Cart? Cart { get; set; }
        public string? DeliveryId { get; set; } // Made optional
        public Delivery? Delivery { get; set; }
        public string? ServiceUsageId { get; set; }
        public string? OrderHistoryId { get; set; }
        public List<OrderItem>? OrderItems { get; set; } = new List<OrderItem>();
        public OrderHistory? OrderHistory { get; set; }
        public ServiceUsage? ServiceUsage { get; set; }
    }
}
    