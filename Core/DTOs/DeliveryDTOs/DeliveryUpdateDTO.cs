using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.DeliveryDTOs
{
    public class DeliveryUpdateDTO
    {
        public string? TrackingNumber { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerName { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
        public string? DeliveryStatus { get; set; }
        public string? Notes { get; set; }
        public decimal? DeliveryFee { get; set; }
        public string? DeliveryPersonId { get; set; }
    }
} 