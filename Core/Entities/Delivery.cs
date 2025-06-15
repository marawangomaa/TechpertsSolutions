using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.Entities
{
    public class Delivery
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        
        public Order? Order { get; set; }
        public ICollection<Order>? Orders { get; set; }
        
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        
        public ICollection<Customer>? Customers { get; set; }
        public ICollection<TechCompany>? TechCompanies { get; set; }
    }
}
