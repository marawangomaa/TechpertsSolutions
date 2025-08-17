using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.NotificationDTOs
{
    public class CreateNotificationDTO
    {
        public string ReceiverUserId { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public NotificationType Type { get; set; }

        public string? RelatedEntityId { get; set; }

        public string? RelatedEntityType { get; set; }
    }
}
