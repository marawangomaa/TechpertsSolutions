using Core.Enums;

namespace TechpertsSolutions.Core.Entities
{
    public class Notification : BaseEntity
    {
        public string ReceiverUserId { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; } = false;
        public string? RelatedEntityId { get; set; } // ID of the related entity (Order, Product, etc.)
        public string? RelatedEntityType { get; set; } // Type of the related entity
        
        // Navigation properties
        public AppUser? Receiver { get; set; }
    }
} 
