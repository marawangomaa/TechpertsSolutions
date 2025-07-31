using Core.DTOs.NotificationDTOs;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Core.Enums;
using Core.Utilities;

namespace TechpertsSolutions.Controllers
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

        [HttpGet("my-notifications")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyNotifications([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated");
            }

            var result = await _notificationService.GetUserNotificationsAsync(userId, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("count")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetNotificationCount()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated");
            }

            var result = await _notificationService.GetNotificationCountAsync(userId);
            return Ok(result);
        }

        [HttpGet("{notificationId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetNotificationById(string notificationId)
        {
            if (string.IsNullOrEmpty(notificationId))
            {
                return BadRequest("Notification ID is required");
            }

            var result = await _notificationService.GetNotificationByIdAsync(notificationId);
            return Ok(result);
        }

        [HttpPost("mark-as-read/{notificationId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarkAsRead(string notificationId)
        {
            if (string.IsNullOrEmpty(notificationId))
            {
                return BadRequest("Notification ID is required");
            }

            var result = await _notificationService.MarkAsReadAsync(notificationId);
            return Ok(result);
        }

        [HttpPost("mark-all-as-read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated");
            }

            var result = await _notificationService.MarkAllAsReadAsync(userId);
            return Ok(result);
        }

        [HttpDelete("{notificationId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteNotification(string notificationId)
        {
            if (string.IsNullOrEmpty(notificationId))
            {
                return BadRequest("Notification ID is required");
            }

            var result = await _notificationService.DeleteNotificationAsync(notificationId);
            return Ok(result);
        }

        [HttpPost("send")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> SendNotification([FromBody] CreateNotificationDTO dto, [FromQuery] NotificationType type)
        {
            if (dto == null)
            {
                return BadRequest("Notification data is required");
            }

            var result = await _notificationService.SendNotificationAsync(
                dto.ReceiverUserId, 
                dto.Message, 
                type, 
                dto.RelatedEntityId, 
                dto.RelatedEntityType);

            return Ok(result);
        }

        [HttpPost("send-to-role")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> SendNotificationToRole([FromBody] string message, [FromQuery] RoleType roleType, [FromQuery] NotificationType type, [FromQuery] string? relatedEntityId = null, [FromQuery] string? relatedEntityType = null)
        {
            if (string.IsNullOrEmpty(message))
            {
                return BadRequest("Message is required");
            }

            var result = await _notificationService.SendNotificationToRoleAsync(
                roleType.GetStringValue(), 
                message, 
                type, 
                relatedEntityId, 
                relatedEntityType);

            return Ok(result);
        }

        [HttpPost("send-to-multiple")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> SendNotificationToMultipleUsers([FromBody] SendToMultipleUsersNotificationDTO dto, [FromQuery] NotificationType type)
        {
            if (dto == null)
            {
                return BadRequest("Notification data is required");
            }

            if (dto.UserIds == null || !dto.UserIds.Any())
            {
                return BadRequest("User IDs list is required and cannot be empty");
            }

            if (string.IsNullOrEmpty(dto.Message))
            {
                return BadRequest("Message is required");
            }

            var result = await _notificationService.SendNotificationToMultipleUsersAsync(
                dto.UserIds, 
                dto.Message, 
                type, 
                dto.RelatedEntityId, 
                dto.RelatedEntityType);

            return Ok(result);
        }
    }
} 
