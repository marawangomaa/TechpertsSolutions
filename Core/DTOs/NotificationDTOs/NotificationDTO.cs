using Core.Enums;

namespace Core.DTOs.NotificationDTOs
{
    public class NotificationDTO
    {
        public string Id { get; set; } = string.Empty;
        
        public string ReceiverUserId { get; set; } = string.Empty;
        
        public string Message { get; set; } = string.Empty;
        
        public NotificationType Type { get; set; }
        
        public bool IsRead { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public string? RelatedEntityId { get; set; }
        
        public string? RelatedEntityType { get; set; }
        
        public string? ReceiverName { get; set; }
    }
} 
