using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechpertsSolutions.Core.Entities
{
    public class Customer : BaseEntity
    {
        public string? City { get; set; }
        public string? Country { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public string RoleId { get; set; }
        public AppRole Role { get; set; }
        public Cart? Cart { get; set; }
        public WishList? WishList { get; set; }
        public List<PCAssembly>? PCAssembly { get; set; } = new List<PCAssembly>();
        public List<Order>? Orders { get; set; } = new List<Order>();
        public List<Maintenance>? Maintenances { get; set; } = new List<Maintenance>();
    }
}
