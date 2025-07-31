using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Enums;

namespace Core.DTOs.ChatDTOs
{
    public class ChatMessageDTO
    {
        public string Id { get; set; } = string.Empty;
        public string ChatRoomId { get; set; } = string.Empty;
        public string SenderId { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public MessageType Type { get; set; }
        public string? FileUrl { get; set; }
        public string? FileName { get; set; }
        public long? FileSize { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public string? ReplyToMessageId { get; set; }
        public ChatMessageDTO? ReplyToMessage { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ChatMessageCreateDTO
    {
        public string ChatRoomId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public MessageType Type { get; set; } = MessageType.Text;
        public string? FileUrl { get; set; }
        public string? FileName { get; set; }
        public long? FileSize { get; set; }
        public string? ReplyToMessageId { get; set; }
    }

    public class ChatMessageUpdateDTO
    {
        public string? Content { get; set; }
        public bool? IsRead { get; set; }
    }
} 