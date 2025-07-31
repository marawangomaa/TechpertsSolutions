using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Enums;

namespace Core.DTOs.CommissionDTOs
{
    public class CommissionTransactionDTO
    {
        public string Id { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string? MaintenanceId { get; set; }
        public string? PCAssemblyId { get; set; }
        public string? DeliveryId { get; set; }
        public ServiceType ServiceType { get; set; }
        public decimal ServiceAmount { get; set; }
        public decimal CommissionAmount { get; set; }
        public decimal VendorPayout { get; set; }
        public decimal PlatformFee { get; set; }
        public string TechCompanyId { get; set; } = string.Empty;
        public string? DeliveryPersonId { get; set; }
        public CommissionStatus Status { get; set; }
        public DateTime? PayoutDate { get; set; }
        public string? PayoutReference { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CommissionTransactionCreateDTO
    {
        public string OrderId { get; set; } = string.Empty;
        public string? MaintenanceId { get; set; }
        public string? PCAssemblyId { get; set; }
        public string? DeliveryId { get; set; }
        public ServiceType ServiceType { get; set; }
        public decimal ServiceAmount { get; set; }
        public string TechCompanyId { get; set; } = string.Empty;
        public string? DeliveryPersonId { get; set; }
    }

    public class CommissionSummaryDTO
    {
        public string TechCompanyId { get; set; } = string.Empty;
        public string TechCompanyName { get; set; } = string.Empty;
        public decimal TotalEarnings { get; set; }
        public decimal TotalCommission { get; set; }
        public decimal TotalPayout { get; set; }
        public int TotalTransactions { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }
} 