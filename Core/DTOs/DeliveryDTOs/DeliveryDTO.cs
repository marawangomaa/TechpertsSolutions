using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.DTOs.DeliveryDTOs
{
    public class DeliveryDTO
    {
        public string Id { get; set; } = null!;
        public string? TrackingNumber { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerName { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
        public string DeliveryStatus { get; set; } = "Pending";
        public string? Notes { get; set; }
        public decimal? DeliveryFee { get; set; }
        public string? DeliveryPersonId { get; set; }
        public string? CustomerId { get; set; }
    }
}
