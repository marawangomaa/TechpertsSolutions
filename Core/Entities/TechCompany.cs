using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechpertsSolutions.Core.Entities
{
    public class TechCompany
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public ICollection<Maintenance>? Maintenances { get; set; }
        public ICollection<Delivery>? Deliveries { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}
