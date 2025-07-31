using Core.Enums.Attributes;

namespace Core.Enums
{
    public enum OrderStatus
    {
        [StringValue("Ordered")]
        Ordered = 0,
        
        [StringValue("Pending")]
        Pending = 1,
        
        [StringValue("InProgress")]
        InProgress = 2,
        
        [StringValue("Approved")]
        Approved = 3,
        
        [StringValue("Dispatched")]
        Dispatched = 4,
        
        [StringValue("Delivered")]
        Delivered = 5,
        
        [StringValue("Cancelled")]
        Cancelled = 6,
        
        [StringValue("Rejected")]
        Rejected = 7
    }
} 
