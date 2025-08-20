using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechpertsSolutions.Core.Entities
{
    public class TechCompany : BaseEntity
    {
        public decimal? Rating { get; set; }
        public string? Description { get; set; }
        public string UserId { get; set; }
        public AppUser? User { get; set; }
        public string RoleId { get; set; }
        public AppRole? Role { get; set; }
        
        // Role-specific dashboard data
        public List<Maintenance>? Maintenances { get; set; } = new List<Maintenance>();
        public List<Delivery>? Deliveries { get; set; } = new List<Delivery>();
        public List<Product>? Products { get; set; } = new List<Product>();
        public List<PCAssembly>? PCAssemblies { get; set; } = new List<PCAssembly>();
    }
}
