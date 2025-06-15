using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechpertsSolutions.Core.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public int CartId { get; set; }
        public Cart? _Cart { get; set; }
        public int SalesManagerId { get; set; }
        public SalesManager SalesManager { get; set; }
        public ICollection<OrderItem>? orderItems { get; set; }
        public OrderHistory? OrderHistory { get; set; }
        public ServiceUsage? ServiceUsage { get; set; }
    }
}
