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
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? City { get; set; }
        public string? Country { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public string RoleId { get; set; }
        public AppRole Role { get; set; }
        public Cart? Cart { get; set; }
        public WishList? WishList { get; set; }
        public ICollection<PCAssembly>? PCAssembly { get; set; }
        public ICollection<Order>? Orders { get; set; }
        public ICollection<Maintenance>? Maintenances { get; set; }
        public string? DeliveryId { get; set; }
        public Delivery? Delivery { get; set; }
    }
    
}
