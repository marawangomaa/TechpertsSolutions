using Core.DTOs.NotificationDTOs;
using Core.Interfaces;
using Microsoft.AspNetCore.SignalR;
using TechpertsSolutions.Hubs;

namespace TechpertsSolutions.Services
{
    public class NotificationHubService : INotificationHub
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationHubService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotificationAsync(string userId, NotificationDTO notification)
        {
            try
            {
                await _hubContext.Clients.Group($"User_{userId}").SendAsync("ReceiveNotification", notification);
            }
            catch (Exception ex)
            {
                // Log the error but don't fail the notification creation
                Console.WriteLine($"Error sending real-time notification: {ex.Message}");
            }
        }
    }
} 
