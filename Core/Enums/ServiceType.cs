using Core.Enums.Attributes;

namespace Core.Enums
{
    public enum ServiceType
    {
        [StringValue("PCBuild")]
        PCBuild,
        [StringValue("Maintenance")]
        Maintenance,
        [StringValue("Delivery")]
        Delivery
    }
}
