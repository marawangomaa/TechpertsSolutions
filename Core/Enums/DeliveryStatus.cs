using Core.Enums.Attributes;

namespace Core.Enums
{
    public enum DeliveryStatus
    {
        [StringValue("Pending")]
        Pending = 0,
        
        [StringValue("Assigned")]
        Assigned = 1,
        
        [StringValue("PickedUp")]
        PickedUp = 2,
        
        [StringValue("Delivered")]
        Delivered = 3,
        
        [StringValue("Cancelled")]
        Cancelled = 4,
        
        [StringValue("Failed")]
        Failed = 5
    }
} 