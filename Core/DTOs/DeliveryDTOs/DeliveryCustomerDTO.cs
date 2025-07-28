using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.DeliveryDTOs
{
    public class DeliveryCustomerDTO
    {
        public string Id { get; set; } = null!;
        public string? City { get; set; }
        public string? Country { get; set; }
        public string UserFullName { get; set; } = null!;
    }
}
