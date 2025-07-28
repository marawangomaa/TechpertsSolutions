using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.Entities
{
    public class Delivery : BaseEntity
    {
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
        
        
        public DeliveryPerson? DeliveryPerson { get; set; }
        public Customer? Customer { get; set; }
        public List<TechCompany>? TechCompanies { get; set; } = new List<TechCompany>();
    }
}
