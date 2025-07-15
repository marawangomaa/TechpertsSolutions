using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Delivery
{
    public class DeliveryDetailsDTO
    {
        public string Id { get; set; } = null!;

        public List<DeliveryCustomerDTO>? Customers { get; set; }
        public List<DeliveryOrderDTO>? Orders { get; set; }
        public List<DeliveryTechCompanyDTO>? TechCompanies { get; set; }
    }
}
