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

    public class CreateNotificationDTO
    {
        public string ReceiverUserId { get; set; } = string.Empty;
        
        public string Message { get; set; } = string.Empty;
        
        public NotificationType Type { get; set; }
        
        public string? RelatedEntityId { get; set; }
        
        public string? RelatedEntityType { get; set; }
    }

    public class NotificationReadDTO
    {
        public string Id { get; set; } = string.Empty;
        
        public string Message { get; set; } = string.Empty;
        
        public NotificationType Type { get; set; }
        
        public bool IsRead { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public string? RelatedEntityId { get; set; }
        
        public string? RelatedEntityType { get; set; }
    }

    public class MarkAsReadDTO
    {
        public string NotificationId { get; set; } = string.Empty;
    }

    public class MarkAllAsReadDTO
    {
        public string UserId { get; set; } = string.Empty;
    }

    public class NotificationCountDTO
    {
        public int TotalCount { get; set; }
        
        public int UnreadCount { get; set; }
    }
} 
