using Core.DTOs.NotificationDTOs;

namespace Core.Interfaces
{
    public interface INotificationHub
    {
        Task SendNotificationAsync(string userId, NotificationDTO notification);
    }
} 
