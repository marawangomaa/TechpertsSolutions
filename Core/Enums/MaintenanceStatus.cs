using Core.Enums.Attributes;

namespace Core.Enums
{
    public enum MaintenanceStatus
    {
        [StringValue("Requested")]
        Requested = 0,
        
        [StringValue("Accepted")]
        Accepted = 1,
        
        [StringValue("InProgress")]
        InProgress = 2,
        
        [StringValue("Completed")]
        Completed = 3,
        
        [StringValue("Cancelled")]
        Cancelled = 4,
        
        [StringValue("Rejected")]
        Rejected = 5
    }
} 