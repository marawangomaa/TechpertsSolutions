using Core.Entities;
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
        public string Status { get; set; }
        public string CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public string CartId { get; set; }
        public Cart? _Cart { get; set; }
        public string DeliveryId { get; set; }
        public Delivery? Delivery { get; set; }
        public string SalesManagerId { get; set; }
        public string ServiceUsageId { get; set; }
        public SalesManager SalesManager { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }
        public OrderHistory? OrderHistory { get; set; }
        public ServiceUsage? ServiceUsage { get; set; }
    }
}
    