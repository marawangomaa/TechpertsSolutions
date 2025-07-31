using Core.Enums.Attributes;

namespace Core.Enums
{
    public enum NotificationType
    {
        [StringValue("OrderCreated")]
        OrderCreated = 0,
        
        [StringValue("ProductAdded")]
        ProductAdded = 1,
        
        [StringValue("ProductApproved")]
        ProductApproved = 2,
        
        [StringValue("ProductRejected")]
        ProductRejected = 3,
        
        [StringValue("ProductPending")]
        ProductPending = 4,
        
        [StringValue("OrderStatusChanged")]
        OrderStatusChanged = 5,
        
        [StringValue("OrderAssignedToDelivery")]
        OrderAssignedToDelivery = 6,
        
        [StringValue("MaintenanceRequestCreated")]
        MaintenanceRequestCreated = 7,
        
        [StringValue("MaintenanceRequestAccepted")]
        MaintenanceRequestAccepted = 8,
        
        [StringValue("MaintenanceRequestCompleted")]
        MaintenanceRequestCompleted = 9,
        
        [StringValue("DeliveryAssigned")]
        DeliveryAssigned = 10,
        
        [StringValue("DeliveryCompleted")]
        DeliveryCompleted = 11,
        
        [StringValue("SystemAlert")]
        SystemAlert = 12
    }
} 
