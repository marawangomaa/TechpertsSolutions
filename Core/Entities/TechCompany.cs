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
<<<<<<< HEAD

=======
>>>>>>> f87b149283b60c7ec0dfab29fe049911a598a8c0
        public string? MapLocation { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public string RoleId { get; set; }
        public AppRole Role { get; set; }
        public ICollection<Maintenance>? Maintenances { get; set; }
        public ICollection<Delivery>? Deliveries { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}
