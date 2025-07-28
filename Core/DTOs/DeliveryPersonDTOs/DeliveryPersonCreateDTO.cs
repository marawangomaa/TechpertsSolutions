using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.DeliveryPersonDTOs
{
    public class DeliveryPersonCreateDTO
    {
        [Required]
        public string UserId { get; set; }
        
        [Required]
        public string RoleId { get; set; }
        
        public string? VehicleNumber { get; set; }
        public string? VehicleType { get; set; }
        public string? PhoneNumber { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }
} 
