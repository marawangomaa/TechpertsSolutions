using Core.DTOs.NotificationDTOs;
using Core.Enums;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserNotifications(string userId, int pageNumber = 1, int pageSize = 20)
        {
            var result = await _notificationService.GetUserNotificationsAsync(userId, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{notificationId}")]
        public async Task<IActionResult> GetNotificationById(string notificationId)
        {
            var result = await _notificationService.GetNotificationByIdAsync(notificationId);
            return Ok(result);
        }

        [HttpGet("count/{userId}")]
        public async Task<IActionResult> GetNotificationCount(string userId)
        {
            var result = await _notificationService.GetNotificationCountAsync(userId);
            return Ok(result);
        }

        [HttpPost("mark-read/{notificationId}")]
        public async Task<IActionResult> MarkAsRead(string notificationId)
        {
            var result = await _notificationService.MarkAsReadAsync(notificationId);
            return Ok(result);
        }

        [HttpPost("mark-all-read/{userId}")]
        public async Task<IActionResult> MarkAllAsRead(string userId)
        {
            var result = await _notificationService.MarkAllAsReadAsync(userId);
            return Ok(result);
        }

        [HttpDelete("{notificationId}")]
        public async Task<IActionResult> DeleteNotification(string notificationId)
        {
            var result = await _notificationService.DeleteNotificationAsync(notificationId);
            return Ok(result);
        }

        [HttpPost("send/{receiverUserId}")]
        public async Task<IActionResult> SendNotification(string receiverUserId, [FromForm] SendNotificationRequest request)
        {
            var result = await _notificationService.SendNotificationAsync(
                receiverUserId,
                request.Type,
                request.RelatedEntityId,
                request.RelatedEntityType,
                request.Args
            );
            return Ok(result);
        }

        [HttpPost("send/multiple")]
        public async Task<IActionResult> SendNotificationsToMultipleUsers([FromBody] SendMultipleNotificationsRequest request)
        {
            var result = await _notificationService.SendNotificationsToMultipleUsers(
                request.UserIds,
                request.Type,
                request.RelatedEntityId,
                request.RelatedEntityType,
                request.Args
            );
            return Ok(result);
        }

        [HttpPost("send/role/{roleName}")]
        public async Task<IActionResult> SendNotificationToRole(string roleName, [FromForm] SendNotificationToRoleRequest request)
        {
            var result = await _notificationService.SendNotificationToRoleAsync(
                roleName,
                request.Type,
                request.RelatedEntityId,
                request.RelatedEntityType,
                request.Args
            );
            return Ok(result);
        }
    }
}
