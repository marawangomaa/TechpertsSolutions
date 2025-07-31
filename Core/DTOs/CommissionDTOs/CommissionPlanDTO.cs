using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.CommissionDTOs
{
    public class CommissionPlanDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal ProductSaleCommission { get; set; }
        public decimal MaintenanceCommission { get; set; }
        public decimal PCAssemblyCommission { get; set; }
        public decimal DeliveryCommission { get; set; }
        public decimal MonthlySubscriptionFee { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
    }

    public class CommissionPlanCreateDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal ProductSaleCommission { get; set; }
        public decimal MaintenanceCommission { get; set; }
        public decimal PCAssemblyCommission { get; set; }
        public decimal DeliveryCommission { get; set; }
        public decimal MonthlySubscriptionFee { get; set; }
        public bool IsDefault { get; set; }
    }

    public class CommissionPlanUpdateDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? ProductSaleCommission { get; set; }
        public decimal? MaintenanceCommission { get; set; }
        public decimal? PCAssemblyCommission { get; set; }
        public decimal? DeliveryCommission { get; set; }
        public decimal? MonthlySubscriptionFee { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDefault { get; set; }
    }
} 