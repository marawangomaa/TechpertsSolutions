using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;
using Core.Enums;

namespace TechpertsSolutions.Core.Entities
{
    public class CommissionTransaction : BaseEntity
    {
        public string OrderId { get; set; } = string.Empty;
        public Order? Order { get; set; }
        
        public string? MaintenanceId { get; set; }
        public Maintenance? Maintenance { get; set; }
        
        public string? PCAssemblyId { get; set; }
        public PCAssembly? PCAssembly { get; set; }
        
        public string? DeliveryId { get; set; }
        public Delivery? Delivery { get; set; }
        
        public ServiceType ServiceType { get; set; }
        public decimal ServiceAmount { get; set; }
        public decimal CommissionAmount { get; set; }
        public decimal VendorPayout { get; set; }
        public decimal PlatformFee { get; set; }
        
        public string? TechCompanyId { get; set; } = string.Empty;
        public TechCompany? TechCompany { get; set; }
        
        public string? DeliveryPersonId { get; set; }
        public DeliveryPerson? DeliveryPerson { get; set; }
        
        public CommissionStatus Status { get; set; } = CommissionStatus.Pending;
        public DateTime? PayoutDate { get; set; }
        public string? PayoutReference { get; set; }
    }
} 