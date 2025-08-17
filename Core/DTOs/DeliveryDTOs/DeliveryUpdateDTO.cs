using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.DeliveryDTOs
{
    public class DeliveryUpdateDTO
    {
        public DeliveryStatus Status { get; set; }
        public decimal? DeliveryFee { get; set; }
    }
} 
