using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace TechpertsSolutions.Core.Entities
{
    public class OrderItem : BaseEntity
    {
        public string ProductId { get; set; }
        public Product? Product { get; set; }
        public string OrderId { get; set; }
        public Order? Order { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal ItemTotal { get; set; }
    }
}
