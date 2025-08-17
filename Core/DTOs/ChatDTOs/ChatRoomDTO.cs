using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Enums;

namespace Core.DTOs.ChatDTOs
{
    public class ChatRoomDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? OrderId { get; set; }
        public string? MaintenanceId { get; set; }
        public string? PCAssemblyId { get; set; }
        public string? DeliveryId { get; set; }
        //public ChatRoomType Type { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public int UnreadMessageCount { get; set; }
        public List<ChatParticipantDTO>? Participants { get; set; }
    }

    public class ChatRoomCreateDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? OrderId { get; set; }
        public string? MaintenanceId { get; set; }
        public string? PCAssemblyId { get; set; }
        public string? DeliveryId { get; set; }
        //public ChatRoomType Type { get; set; }
        public List<string> ParticipantIds { get; set; } = new List<string>();
    }

    public class ChatRoomUpdateDTO
    {
        public string? Name { get; set; }
        public bool? IsActive { get; set; }
    }
} 