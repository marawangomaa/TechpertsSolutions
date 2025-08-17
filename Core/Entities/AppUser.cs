using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechpertsSolutions.Core.Entities
{
    public class AppUser : IdentityUser, IEntity
    {
        public string FullName { get; set; }
        public string Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? ProfilePhotoUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties for role-specific entities
        public Customer? Customer { get; set; }
        public Admin? Admin { get; set; }
        public TechCompany? TechCompany { get; set; }
        public DeliveryPerson? DeliveryPerson { get; set; }
        public List<PrivateMessage> SentMessages { get; set; } = new List<PrivateMessage>();
        public List<PrivateMessage> ReceivedMessages { get; set; } = new List<PrivateMessage>();
    }
}

