using Core.DTOs.NotificationDTOs;
using Core.Enums;
using Core.DTOs;

namespace Core.Interfaces.Services
{
    public interface INotificationService
    {
        Task<GeneralResponse<NotificationDTO>> SendNotificationAsync(string receiverUserId, string message, NotificationType type, string? relatedEntityId = null, string? relatedEntityType = null);
        Task<GeneralResponse<IEnumerable<NotificationDTO>>> GetUserNotificationsAsync(string userId, int pageNumber = 1, int pageSize = 20);
        Task<GeneralResponse<NotificationDTO>> GetNotificationByIdAsync(string notificationId);
        Task<GeneralResponse<bool>> MarkAsReadAsync(string notificationId);
        Task<GeneralResponse<bool>> MarkAllAsReadAsync(string userId);
        Task<GeneralResponse<bool>> DeleteNotificationAsync(string notificationId);
        Task<GeneralResponse<NotificationCountDTO>> GetNotificationCountAsync(string userId);
        Task<GeneralResponse<bool>> SendNotificationToRoleAsync(string roleName, string message, NotificationType type, string? relatedEntityId = null, string? relatedEntityType = null);
        Task<GeneralResponse<bool>> SendNotificationToMultipleUsersAsync(List<string> userIds, string message, NotificationType type, string? relatedEntityId = null, string? relatedEntityType = null);
    }
} 
