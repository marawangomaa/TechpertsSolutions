using Core.DTOs;
using Core.DTOs.NotificationDTOs;
using Core.Enums;
using Core.Factories;
using Core.Interfaces;
using Core.Interfaces.Services;
using Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using TechpertsSolutions.Core.Entities;

namespace Service
{
    public class NotificationService : INotificationService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IHubContext<NotificationsHub> _hubContext;
        private readonly IRepository<Notification> _notificationRepo;

        public NotificationService(
            IRepository<Notification> notificationRepo,
            UserManager<AppUser> userManager,
            IHubContext<NotificationsHub> hubContext
        )
        {
            _notificationRepo = notificationRepo;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        // ------------------ READ METHODS ------------------

        public async Task<GeneralResponse<IEnumerable<NotificationDTO>>> GetUserNotificationsAsync(
            string userId,
            int pageNumber = 1,
            int pageSize = 20
        )
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    return new GeneralResponse<IEnumerable<NotificationDTO>>
                    {
                        Success = false,
                        Message = "User ID cannot be null or empty.",
                        Data = null,
                    };

                var notifications = await _notificationRepo.FindWithIncludesAsync(
                    n => n.ReceiverUserId == userId,
                    n => n.Receiver
                );

                var sortedNotifications = notifications
                    .OrderByDescending(n => n.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(MapToDTO)
                    .ToList();

                return new GeneralResponse<IEnumerable<NotificationDTO>>
                {
                    Success = true,
                    Message = "Notifications retrieved successfully.",
                    Data = sortedNotifications,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<IEnumerable<NotificationDTO>>
                {
                    Success = false,
                    Message = $"An error occurred while retrieving notifications: {ex.Message}",
                    Data = null,
                };
            }
        }

        public async Task<GeneralResponse<NotificationDTO>> GetNotificationByIdAsync(
            string notificationId
        )
        {
            try
            {
                if (string.IsNullOrWhiteSpace(notificationId))
                    return new GeneralResponse<NotificationDTO>
                    {
                        Success = false,
                        Message = "Notification ID cannot be null or empty.",
                        Data = null,
                    };

                var notification = await _notificationRepo.GetByIdWithIncludesAsync(
                    notificationId,
                    n => n.Receiver
                );

                if (notification == null)
                    return new GeneralResponse<NotificationDTO>
                    {
                        Success = false,
                        Message = "Notification not found.",
                        Data = null,
                    };

                return new GeneralResponse<NotificationDTO>
                {
                    Success = true,
                    Message = "Notification retrieved successfully.",
                    Data = MapToDTO(notification),
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<NotificationDTO>
                {
                    Success = false,
                    Message = $"An error occurred while retrieving notification: {ex.Message}",
                    Data = null,
                };
            }
        }

        public async Task<GeneralResponse<NotificationCountDTO>> GetNotificationCountAsync(
            string userId
        )
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    return new GeneralResponse<NotificationCountDTO>
                    {
                        Success = false,
                        Message = "User ID cannot be null or empty.",
                        Data = null,
                    };

                var all = await _notificationRepo.FindAsync(n => n.ReceiverUserId == userId);
                var unread = all.Where(n => !n.IsRead).ToList();

                return new GeneralResponse<NotificationCountDTO>
                {
                    Success = true,
                    Message = "Notification count retrieved successfully.",
                    Data = new NotificationCountDTO
                    {
                        TotalCount = all.Count(),
                        UnreadCount = unread.Count(),
                    },
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<NotificationCountDTO>
                {
                    Success = false,
                    Message =
                        $"An error occurred while retrieving notification count: {ex.Message}",
                    Data = null,
                };
            }
        }

        // ------------------ MARK / DELETE ------------------

        public async Task<GeneralResponse<bool>> MarkAsReadAsync(string notificationId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(notificationId))
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Notification ID cannot be null or empty.",
                        Data = false,
                    };

                var notification = await _notificationRepo.GetByIdAsync(notificationId);
                if (notification == null)
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Notification not found.",
                        Data = false,
                    };

                notification.IsRead = true;
                _notificationRepo.Update(notification);
                await _notificationRepo.SaveChangesAsync();

                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = "Notification marked as read successfully.",
                    Data = true,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = $"An error occurred while marking notification as read: {ex.Message}",
                    Data = false,
                };
            }
        }

        public async Task<GeneralResponse<bool>> MarkAllAsReadAsync(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "User ID cannot be null or empty.",
                        Data = false,
                    };

                var notifications = await _notificationRepo.FindAsync(n =>
                    n.ReceiverUserId == userId && !n.IsRead
                );
                foreach (var n in notifications)
                    n.IsRead = true;

                await _notificationRepo.SaveChangesAsync();

                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = "All notifications marked as read successfully.",
                    Data = true,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message =
                        $"An error occurred while marking all notifications as read: {ex.Message}",
                    Data = false,
                };
            }
        }

        public async Task<GeneralResponse<bool>> DeleteNotificationAsync(string notificationId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(notificationId))
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Notification ID cannot be null or empty.",
                        Data = false,
                    };

                var notification = await _notificationRepo.GetByIdAsync(notificationId);
                if (notification == null)
                    return new GeneralResponse<bool>
                    {
                        Success = false,
                        Message = "Notification not found.",
                        Data = false,
                    };

                _notificationRepo.Remove(notification);
                await _notificationRepo.SaveChangesAsync();

                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = "Notification deleted successfully.",
                    Data = true,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = $"An error occurred while deleting notification: {ex.Message}",
                    Data = false,
                };
            }
        }

        // ------------------ SEND NOTIFICATIONS ------------------

        public async Task<GeneralResponse<NotificationDTO>> SendNotificationAsync(
            string receiverUserId,
            NotificationType type,
            string? relatedEntityId = null,
            string? relatedEntityType = null,
            params object[] args
        )
        {
            try
            {
                var notification = NotificationsFactory.Create(
                    receiverUserId,
                    type,
                    relatedEntityId,
                    relatedEntityType,
                    args ?? Array.Empty<object>()
                );
                var dto = await SaveAndPushAsync(notification);

                return new GeneralResponse<NotificationDTO>
                {
                    Success = true,
                    Message = "Notification sent successfully.",
                    Data = dto,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<NotificationDTO>
                {
                    Success = false,
                    Message = $"Failed to send notification: {ex.Message}",
                    Data = null,
                };
            }
        }

        public async Task<GeneralResponse<bool>> SendNotificationsToMultipleUsers(
            IEnumerable<string> userIds,
            NotificationType type,
            string? relatedEntityId = null,
            string? relatedEntityType = null,
            params object[] args
        )
        {
            try
            {
                var notifications = NotificationsFactory.CreateForUsers(
                    userIds,
                    type,
                    relatedEntityId,
                    relatedEntityType,
                    args ?? Array.Empty<object>()
                );

                foreach (var notification in notifications)
                    await SaveAndPushAsync(notification);

                return new GeneralResponse<bool>
                {
                    Success = true,
                    Message = $"Notifications sent successfully to {notifications.Count} users.",
                    Data = true,
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = $"Failed to send notifications: {ex.Message}",
                    Data = false,
                };
            }
        }

        public async Task<GeneralResponse<bool>> SendNotificationToRoleAsync(
            string roleName,
            NotificationType type,
            string? relatedEntityId = null,
            string? relatedEntityType = null,
            params object[] args
        )
        {
            try
            {
                var users = await _userManager.GetUsersInRoleAsync(roleName);
                var userIds = users.Select(u => u.Id);

                return await SendNotificationsToMultipleUsers(
                    userIds,
                    type,
                    relatedEntityId,
                    relatedEntityType,
                    args
                );
            }
            catch (Exception ex)
            {
                return new GeneralResponse<bool>
                {
                    Success = false,
                    Message = $"Failed to send notifications to role: {ex.Message}",
                    Data = false,
                };
            }
        }

        // ------------------ PRIVATE HELPERS ------------------

        private async Task<NotificationDTO> SaveAndPushAsync(Notification notification)
        {
            await _notificationRepo.AddAsync(notification);
            await _notificationRepo.SaveChangesAsync();

            await SendRealTimeNotificationAsync(notification.ReceiverUserId, notification);

            return NotificationsFactory.ToDTO(notification);
        }

        private async Task SendRealTimeNotificationAsync(string userId, Notification notification)
        {
            try
            {
                await _hubContext
                    .Clients.Group($"User_{userId}")
                    .SendAsync("ReceiveNotification", NotificationsFactory.ToDTO(notification));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR error: {ex.Message}");
            }
        }

        private NotificationDTO MapToDTO(Notification notification)
        {
            return NotificationsFactory.ToDTO(notification);
        }
    }
}
