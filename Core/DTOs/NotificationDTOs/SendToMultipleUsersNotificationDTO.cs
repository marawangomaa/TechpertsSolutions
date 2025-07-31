using Core.Enums;

namespace Core.DTOs.NotificationDTOs
{
    public class SendToMultipleUsersNotificationDTO
    {
        public List<string> UserIds { get; set; } = new List<string>();
        
        public string Message { get; set; } = string.Empty;
        
        public NotificationType Type { get; set; }
        
        public string? RelatedEntityId { get; set; }
        
        public string? RelatedEntityType { get; set; }
    }
} 