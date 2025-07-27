using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechpertsSolutions.Core.Entities
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }
        public string Address { get; set; }
        public Customer? Customer { get; set; }
        public Admin? Admin { get; set; }
        public TechCompany? TechCompany { get; set; }
        public DeliveryPerson? DeliveryPerson { get; set; }
    }
}

