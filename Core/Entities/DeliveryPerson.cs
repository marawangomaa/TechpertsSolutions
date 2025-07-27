using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechpertsSolutions.Core.Entities
{
    public class DeliveryPerson : BaseEntity
    {
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public string RoleId { get; set; }
        public AppRole Role { get; set; }
        public string? VehicleNumber { get; set; }
        public string? VehicleType { get; set; }
        public string? PhoneNumber { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public bool IsAvailable { get; set; } = true;
        public List<Delivery>? Deliveries { get; set; } = new List<Delivery>();
    }
} 