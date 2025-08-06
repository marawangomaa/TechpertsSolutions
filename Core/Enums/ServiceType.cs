using Core.Enums.Attributes;

namespace Core.Enums
{
    public enum ServiceType
    {
        [StringValue("ProductSale")]
        ProductSale = 0,
        
        [StringValue("Maintenance")]
        Maintenance = 1,
        
        [StringValue("PCAssembly")]
        PCAssembly = 2,
        
        [StringValue("Delivery")]
        Delivery = 3,
    }
}
