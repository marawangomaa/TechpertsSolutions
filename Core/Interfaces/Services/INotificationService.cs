using Core.DTOs;
using Core.DTOs.NotificationDTOs;
using Core.Enums;

namespace Core.Interfaces.Services
{
    public interface INotificationService
    {
        Task<GeneralResponse<IEnumerable<NotificationDTO>>> GetUserNotificationsAsync(string userId, int pageNumber = 1, int pageSize = 20);
        Task<GeneralResponse<NotificationDTO>> GetNotificationByIdAsync(string notificationId);
        Task<GeneralResponse<NotificationCountDTO>> GetNotificationCountAsync(string userId);

        Task<GeneralResponse<bool>> MarkAsReadAsync(string notificationId);
        Task<GeneralResponse<bool>> MarkAllAsReadAsync(string userId);
        Task<GeneralResponse<bool>> DeleteNotificationAsync(string notificationId);

        Task<GeneralResponse<NotificationDTO>> SendNotificationAsync(string receiverUserId, NotificationType type, string? relatedEntityId = null, string? relatedEntityType = null, params object[] args);
        Task<GeneralResponse<bool>> SendNotificationsToMultipleUsers(IEnumerable<string> userIds, NotificationType type, string? relatedEntityId = null, string? relatedEntityType = null, params object[] args);
        Task<GeneralResponse<bool>> SendNotificationToRoleAsync(string roleName, NotificationType type, string? relatedEntityId = null, string? relatedEntityType = null, params object[] args);
    }
}
