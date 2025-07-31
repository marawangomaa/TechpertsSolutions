using Core.Enums.Attributes;

namespace Core.Enums
{
    public enum ProductPendingStatus
    {
        [StringValue("Pending")]
        Pending = 0,
        
        [StringValue("Approved")]
        Approved = 1,
        
        [StringValue("Rejected")]
        Rejected = 2
    }
}
