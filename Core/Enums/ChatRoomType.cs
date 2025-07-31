using Core.Enums.Attributes;

namespace Core.Enums
{
    public enum ChatRoomType
    {
        [StringValue("OrderSupport")]
        OrderSupport = 0,
        
        [StringValue("MaintenanceSupport")]
        MaintenanceSupport = 1,
        
        [StringValue("PCAssemblySupport")]
        PCAssemblySupport = 2,
        
        [StringValue("DeliverySupport")]
        DeliverySupport = 3,
        
        [StringValue("GeneralSupport")]
        GeneralSupport = 4
    }
} 