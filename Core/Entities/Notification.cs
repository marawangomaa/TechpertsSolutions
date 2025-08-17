using Core.Enums;

namespace TechpertsSolutions.Core.Entities
{
    public class Notification : BaseEntity
    {
        public string ReceiverUserId { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; } = false;
        public string? RelatedEntityId { get; set; }
        public string? RelatedEntityType { get; set; }
        
        public AppUser? Receiver { get; set; }
    }
} 
