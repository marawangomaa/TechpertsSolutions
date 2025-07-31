using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Enums;

namespace Core.DTOs.ChatDTOs
{
    public class ChatParticipantDTO
    {
        public string Id { get; set; } = string.Empty;
        public string ChatRoomId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
        public string? UserProfilePhoto { get; set; }
        public ParticipantRole Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastSeen { get; set; }
        public bool IsTyping { get; set; }
        public DateTime JoinedAt { get; set; }
    }

    public class ChatParticipantCreateDTO
    {
        public string ChatRoomId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public ParticipantRole Role { get; set; }
    }

    public class ChatParticipantUpdateDTO
    {
        public bool? IsActive { get; set; }
        public bool? IsTyping { get; set; }
    }
} 