using Core.Enums.Attributes;

namespace Core.Enums
{
    public enum NotificationType
    {
        // Order-related
        [StringValue("OrderCreated")]
        OrderCreated = 0,

        [StringValue("OrderStatusChanged")]
        OrderStatusChanged = 1,

        [StringValue("OrderAssignedToDelivery")]
        OrderAssignedToDelivery = 2,

        // Product-related
        [StringValue("ProductAdded")]
        ProductAdded = 10,

        [StringValue("ProductApproved")]
        ProductApproved = 11,

        [StringValue("ProductRejected")]
        ProductRejected = 12,

        [StringValue("ProductPending")]
        ProductPending = 13,

        // Maintenance-related
        [StringValue("MaintenanceRequestCreated")]
        MaintenanceRequestCreated = 20,

        [StringValue("MaintenanceRequestAccepted")]
        MaintenanceRequestAccepted = 21,

        [StringValue("MaintenanceRequestCompleted")]
        MaintenanceRequestCompleted = 22,

        // Delivery-related
        [StringValue("DeliveryOfferCreated")]
        DeliveryOfferCreated = 30,

        [StringValue("DeliveryOfferAccepted")]
        DeliveryOfferAccepted = 31,

        [StringValue("DeliveryOfferExpired")] 
        DeliveryOfferExpired = 32,

        [StringValue("DeliveryOfferCanceled")]
        DeliveryOfferCanceled= 33,

        [StringValue("DeliveryOfferDeclined")]
        DeliveryOfferDeclined= 34,

        [StringValue("DeliveryAssigned")]
        DeliveryAssigned = 35,

        [StringValue("DeliveryPickedUp")]
        DeliveryPickedUp = 36,

        [StringValue("DeliveryTrackingUpdated")]
        DeliveryTrackingUpdated = 37,

        [StringValue("DeliveryCompleted")]
        DeliveryCompleted = 38,

        // System
        [StringValue("SystemAlert")]
        SystemAlert = 40
    }
}
