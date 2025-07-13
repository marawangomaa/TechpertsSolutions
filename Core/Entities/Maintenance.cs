using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.Entities
{
    public class Maintenance : BaseEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string WarrantyId { get; set; }
        public Warranty? Warranty { get; set; }

        public string TechCompanyId { get; set; }
        public TechCompany? TechCompany { get; set; }

        public string CustomerId { get; set; }
        public Customer? Customer{ get; set; }

        public string ServiceUsageId { get; set; }
        public ServiceUsage? serviceUsage{ get; set; }
    }
}
