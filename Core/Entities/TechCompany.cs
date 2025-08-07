using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechpertsSolutions.Core.Entities
{
    public class TechCompany : BaseEntity
    {
        // Business-specific properties
        public string? MapLocation { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Website { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        
        public string UserId { get; set; }
        public AppUser? User { get; set; }
        public string RoleId { get; set; }
        public AppRole? Role { get; set; }
        
        public string? CommissionPlanId { get; set; }
        public CommissionPlan? CommissionPlan { get; set; }
        
        // Role-specific dashboard data
        public List<Maintenance>? Maintenances { get; set; } = new List<Maintenance>();
        public List<Delivery>? Deliveries { get; set; } = new List<Delivery>();
        public List<Product>? Products { get; set; } = new List<Product>();
        public List<PCAssembly>? PCAssemblies { get; set; } = new List<PCAssembly>();
    }
}
