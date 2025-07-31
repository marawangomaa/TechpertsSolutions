using Core.Enums.Attributes;

namespace Core.Enums
{
    public enum CommissionStatus
    {
        [StringValue("Pending")]
        Pending = 0,
        
        [StringValue("Processed")]
        Processed = 1,
        
        [StringValue("Paid")]
        Paid = 2,
        
        [StringValue("Failed")]
        Failed = 3,
        
        [StringValue("Refunded")]
        Refunded = 4
    }
} 