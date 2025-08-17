using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.DTOs.DeliveryPersonDTOs
{
    public class DeliveryPersonDTO
    {
        public string Id { get; set; }
        public string? UserFullName { get; set; }
        public string? VehicleNumber { get; set; }
        public string? VehicleType { get; set; }
        public string? PhoneNumber { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public bool IsAvailable { get; set; }
        public AppUser User { get; set; }
    }
} 
