using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;
using Core.Enums;

namespace TechpertsSolutions.Core.Entities
{
    public class PCAssembly : BaseEntity
    {
        public string? Name { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public Customer? Customer { get; set; }
        public string? TechCompanyId { get; set; }
        public TechCompany? TechCompany { get; set; }
        public string ServiceUsageId { get; set; } = string.Empty;
        public ServiceUsage? ServiceUsage { get; set; }
        public PCAssemblyStatus Status { get; set; } = PCAssemblyStatus.Requested;
        public string? Description { get; set; }
        public decimal? Budget { get; set; }
        public decimal? AssemblyFee { get; set; }
        public DateTime? CompletedDate { get; set; }
        public List<PCAssemblyItem>? PCAssemblyItems { get; set; } = new List<PCAssemblyItem>();
    }
}
