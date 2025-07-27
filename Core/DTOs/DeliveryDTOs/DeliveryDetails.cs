using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.DeliveryDTOs
{
    public class DeliveryDetailsDTO
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
        public DeliveryPersonDTO? DeliveryPerson { get; set; }
        public DeliveryOrderDTO? Order { get; set; }
        public List<DeliveryTechCompanyDTO>? TechCompanies { get; set; }
    }
}
