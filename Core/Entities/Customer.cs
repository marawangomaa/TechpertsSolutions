using TechpertsSolutions.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechpertsSolutions.Core.Entities
{
    public class Customer : BaseEntity
    {
        public string UserId { get; set; }
        public AppUser? User { get; set; }
        public string RoleId { get; set; }
        public AppRole? Role { get; set; }
        
        // Role-specific dashboard data
        public Cart? Cart { get; set; }
        public WishList? WishList { get; set; }
        public List<PCAssembly>? PCAssembly { get; set; } = new List<PCAssembly>();
        public List<Order>? Orders { get; set; } = new List<Order>();
        public List<Delivery>? Deliveries { get; set; } = new List<Delivery>();
        public List<Maintenance>? Maintenances { get; set; } = new List<Maintenance>();
    }
}
