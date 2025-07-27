using Core.Enums.Attributes;

namespace Core.Enums
{
    public enum OrderStatus
    {
        [StringValue("Pending")]
        Pending,
        [StringValue("InProgress")]
        InProgress,
        [StringValue("Delivered")]
        Delivered
    }
} 