using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.DTOs.Delivery
{
    public class DeliveryCreateDTO
    {
        public ICollection<Order>? Orders { get; set; }
        public ICollection<Customer>? Customers { get; set; }
        public ICollection<TechCompany>? TechCompanies { get; set; }
    }
}
