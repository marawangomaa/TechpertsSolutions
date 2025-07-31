using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Enums;

namespace TechpertsSolutions.Core.Entities
{
    public class ChatParticipant : BaseEntity
    {
        public string ChatRoomId { get; set; } = string.Empty;
        public ChatRoom? ChatRoom { get; set; }
        
        public string UserId { get; set; } = string.Empty;
        public AppUser? User { get; set; }
        
        public ParticipantRole Role { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? LastSeen { get; set; }
        public bool IsTyping { get; set; } = false;
    }
} 