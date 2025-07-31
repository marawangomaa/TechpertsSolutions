using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Core.Enums;

namespace TechpertsSolutions.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly Dictionary<string, string> _userConnections = new();
        private static readonly Dictionary<string, List<string>> _userChatRooms = new();
        private static readonly Dictionary<string, bool> _typingUsers = new();

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                _userConnections[userId] = Context.ConnectionId;
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                _userConnections.Remove(userId);
                _typingUsers.Remove(userId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinChatRoom(string chatRoomId)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"ChatRoom_{chatRoomId}");
                
                if (!_userChatRooms.ContainsKey(userId))
                    _userChatRooms[userId] = new List<string>();
                
                if (!_userChatRooms[userId].Contains(chatRoomId))
                    _userChatRooms[userId].Add(chatRoomId);
            }
        }

        public async Task LeaveChatRoom(string chatRoomId)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"ChatRoom_{chatRoomId}");
                
                if (_userChatRooms.ContainsKey(userId))
                    _userChatRooms[userId].Remove(chatRoomId);
            }
        }

        public async Task SendMessage(string chatRoomId, string message, string? replyToMessageId = null)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
            
            if (!string.IsNullOrEmpty(userId))
            {
                await Clients.Group($"ChatRoom_{chatRoomId}").SendAsync("ReceiveMessage", new
                {
                    ChatRoomId = chatRoomId,
                    SenderId = userId,
                    SenderName = userName,
                    Content = message,
                    ReplyToMessageId = replyToMessageId,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        public async Task SendFile(string chatRoomId, string fileName, string fileUrl, long fileSize)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
            
            if (!string.IsNullOrEmpty(userId))
            {
                await Clients.Group($"ChatRoom_{chatRoomId}").SendAsync("ReceiveFile", new
                {
                    ChatRoomId = chatRoomId,
                    SenderId = userId,
                    SenderName = userName,
                    FileName = fileName,
                    FileUrl = fileUrl,
                    FileSize = fileSize,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        public async Task SetTypingStatus(string chatRoomId, bool isTyping)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
            
            if (!string.IsNullOrEmpty(userId))
            {
                _typingUsers[userId] = isTyping;
                await Clients.Group($"ChatRoom_{chatRoomId}").SendAsync("UserTyping", new
                {
                    ChatRoomId = chatRoomId,
                    UserId = userId,
                    UserName = userName,
                    IsTyping = isTyping
                });
            }
        }

        public async Task MarkMessageAsRead(string chatRoomId, string messageId)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (!string.IsNullOrEmpty(userId))
            {
                await Clients.Group($"ChatRoom_{chatRoomId}").SendAsync("MessageRead", new
                {
                    ChatRoomId = chatRoomId,
                    MessageId = messageId,
                    ReadByUserId = userId,
                    ReadAt = DateTime.UtcNow
                });
            }
        }

        public static string? GetConnectionId(string userId)
        {
            return _userConnections.TryGetValue(userId, out var connectionId) ? connectionId : null;
        }

        public static List<string> GetConnectionIds(List<string> userIds)
        {
            return userIds.Where(id => _userConnections.ContainsKey(id))
                         .Select(id => _userConnections[id])
                         .ToList();
        }

        public static bool IsUserTyping(string userId)
        {
            return _typingUsers.TryGetValue(userId, out var isTyping) && isTyping;
        }
    }
} 