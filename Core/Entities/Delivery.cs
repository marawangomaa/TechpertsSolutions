using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.Entities
{
    public class Delivery : BaseEntity
    {
        public List<Order>? Orders { get; set; } = new List<Order>();
        public List<Customer>? Customers { get; set; } = new List<Customer>();
        public List<TechCompany> TechCompanies { get; set; } = new List<TechCompany>();

    }
}
