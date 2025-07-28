using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechpertsSolutions.Core.Entities
{
    public class TechCompany : BaseEntity
    {

        public string? MapLocation { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public string RoleId { get; set; }
        public AppRole Role { get; set; }
        public List<Maintenance>? Maintenances { get; set; } = new List<Maintenance>();
        public List<Delivery>? Deliveries { get; set; } = new List<Delivery>();
        public List<Product>? Products { get; set; } = new List<Product>();
    }
}
