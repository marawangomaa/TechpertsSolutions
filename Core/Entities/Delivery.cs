using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;
using Core.Enums;

namespace TechpertsSolutions.Core.Entities
{
    public class Delivery : BaseEntity
    {
        public string? TrackingNumber { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerName { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
        public DeliveryStatus Status { get; set; } = DeliveryStatus.Assigned;
        public string? Notes { get; set; }
        public decimal? DeliveryFee { get; set; }
        public string? PickupAddress { get; set; }
        public DateTime? PickupDate { get; set; }
        public string? DeliveryPersonId { get; set; }
        public string? CustomerId { get; set; }
        public string? OrderId { get; set; }
        public Order? Order { get; set; }
        
        public DeliveryPerson? DeliveryPerson { get; set; }
        public Customer? Customer { get; set; }
        public List<TechCompany>? TechCompanies { get; set; } = new List<TechCompany>();
    }
}
