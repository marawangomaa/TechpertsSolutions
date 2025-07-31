using Core.Enums.Attributes;

namespace Core.Enums
{
    public enum PCAssemblyStatus
    {
        [StringValue("Requested")]
        Requested = 0,
        
        [StringValue("Accepted")]
        Accepted = 1,
        
        [StringValue("Assembled")]
        Assembled = 2,
        
        [StringValue("Ready")]
        Ready = 3,
        
        [StringValue("Completed")]
        Completed = 4,
        
        [StringValue("Cancelled")]
        Cancelled = 5,
        
        [StringValue("Rejected")]
        Rejected = 6
    }
} 