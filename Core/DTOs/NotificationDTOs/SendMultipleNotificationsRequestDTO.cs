using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.NotificationDTOs
{
    public class SendMultipleNotificationsRequest
    {
        public IEnumerable<string> UserIds { get; set; } = new List<string>();
        public NotificationType Type { get; set; }
        public string? RelatedEntityId { get; set; }
        public string? RelatedEntityType { get; set; }
        public object[] Args { get; set; } = Array.Empty<object>();
    }
}
