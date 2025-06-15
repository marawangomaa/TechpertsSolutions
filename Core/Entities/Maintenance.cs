using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.Entities
{
    public class Maintenance
    {
        public int Id { get; set; }
        public int WarrantyId { get; set; }
        public Warranty? Warranty { get; set; }

        public int TechCompanyId { get; set; }
        public TechCompany? TechCompany { get; set; }

        public int CustomerId { get; set; }
        public Customer? Customer{ get; set; }

        public int ServiceUsageId{ get; set; }
        public ServiceUsage? serviceUsage{ get; set; }
    }
}
