using Core.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.DeliveryPersonDTOs
{
    public class DeliveryPersonAdminUpdateDTO
    {
        public string? VehicleNumber { get; set; }
        public string? VehicleType { get; set; }
        public string? PhoneNumber { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public bool? IsAvailable { get; set; }
        public DeliveryPersonStatus AccountStatus { get; set; }
    }
}
