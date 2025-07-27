using Core.Enums.Attributes;

namespace Core.Enums
{
    public enum ProductPendingStatus
    {
        [StringValue("Pending")]
        Pending,
        [StringValue("Approved")]
        Approved,
        [StringValue("Rejected")]
        Rejected
    }
}
