using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using TechpertsSolutions.Core.Entities;

namespace Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatHub : Hub
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IRepository<PrivateMessage> _messageRepo;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(
            UserManager<AppUser> userManager,
            ILogger<ChatHub> logger,
            IRepository<PrivateMessage> messageRepo
        )
        {
            _userManager = userManager;
            _logger = logger;
            _messageRepo = messageRepo;
        }

        //public async Task SendPrivateMessage(string receiverUserId, string messageText)
        //{
        //    var senderUserId = Context.UserIdentifier;

        //    if (string.IsNullOrEmpty(senderUserId))
        //        throw new HubException("Unauthorized");
        //    var sender = await _userManager.FindByIdAsync(senderUserId);
        //    var receiver = await _userManager.FindByIdAsync(receiverUserId);

        //    var senderRoles = await _userManager.GetRolesAsync(sender);
        //    var receiverRoles = await _userManager.GetRolesAsync(receiver);

        //    var message = new PrivateMessage
        //    {
        //        SenderUserId = senderUserId,
        //        ReceiverUserId = receiverUserId,
        //        MessageText = messageText,
        //        SentAt = DateTime.UtcNow,
        //        IsRead = false,
        //        SenderRole = senderRoles.FirstOrDefault() ?? "User",
        //        ReceiverRole = receiverRoles.FirstOrDefault() ?? "User"
        //    };

        //    await _messageRepo.AddAsync(message);
        //    await _messageRepo.SaveChangesAsync();  // make sure this is here
        //    Console.WriteLine($"💾 Message saved: {message.Id}");

        //    // Broadcast to receiver
        //    await Clients.User(receiverUserId).SendAsync("ReceivePrivateMessage", new PrivateMessageDTO
        //    {
        //        Id = message.Id,
        //        SenderUserId = message.SenderUserId,
        //        ReceiverUserId = message.ReceiverUserId,
        //        SenderName = Context.User?.Identity?.Name ?? "Unknown",
        //        ReceiverName = "", // optional, can load if needed
        //        MessageText = message.MessageText,
        //        SentAt = message.SentAt,
        //        IsRead = message.IsRead
        //    });

        //    // Optionally echo back to sender so it appears immediately
        //    await Clients.User(senderUserId).SendAsync("ReceivePrivateMessage", new PrivateMessageDTO
        //    {
        //        Id = message.Id,
        //        SenderUserId = message.SenderUserId,
        //        ReceiverUserId = message.ReceiverUserId,
        //        SenderName = Context.User?.Identity?.Name ?? "Me",
        //        ReceiverName = "",
        //        MessageText = message.MessageText,
        //        SentAt = message.SentAt,
        //        IsRead = message.IsRead
        //    });
        //}

        public async Task SendPrivateMessage(string receiverUserId, string messageText)
        {
            var senderUserId = Context.UserIdentifier;

            if (string.IsNullOrEmpty(senderUserId))
                throw new HubException("Unauthorized");

            // Get sender and receiver users
            var sender = await _userManager.FindByIdAsync(senderUserId);
            var receiver = await _userManager.FindByIdAsync(receiverUserId);

            if (sender == null || receiver == null)
                throw new HubException("User not found");

            // Get roles
            var senderRoles = await _userManager.GetRolesAsync(sender);
            var receiverRoles = await _userManager.GetRolesAsync(receiver);

            // Create message entity
            var message = new PrivateMessage
            {
                SenderUserId = senderUserId,
                ReceiverUserId = receiverUserId,
                MessageText = messageText,
                SentAt = DateTime.UtcNow,
                IsRead = false,
                SenderRole = senderRoles.FirstOrDefault() ?? "User",
                ReceiverRole = receiverRoles.FirstOrDefault() ?? "User"
            };

            // Save to database
            await _messageRepo.AddAsync(message);
            await _messageRepo.SaveChangesAsync();
            Console.WriteLine($"💾 Message saved: {message.Id}");

            // Build DTO for receiver (they see sender’s real name)
            var receiverDto = new PrivateMessageDTO
            {
                Id = message.Id,
                SenderUserId = message.SenderUserId,
                ReceiverUserId = message.ReceiverUserId,
                SenderName = sender.FullName ?? Context.User?.Identity?.Name ?? "Unknown",
                ReceiverName = receiver.FullName ?? "Unknown",
                MessageText = message.MessageText,
                SentAt = message.SentAt,
                IsRead = message.IsRead
            };

            // Send to receiver
            await Clients.User(receiverUserId).SendAsync("ReceivePrivateMessage", receiverDto);

            // Build DTO for sender (they see themselves as "Me")
            var senderDto = new PrivateMessageDTO
            {
                Id = message.Id,
                SenderUserId = message.SenderUserId,
                ReceiverUserId = message.ReceiverUserId,
                SenderName = sender.FullName ??"Me",
                ReceiverName = receiver.FullName ?? "Unknown",
                MessageText = message.MessageText,
                SentAt = message.SentAt,
                IsRead = message.IsRead
            };

            // Echo back to sender
            await Clients.User(senderUserId).SendAsync("ReceivePrivateMessage", senderDto);
        }

        public async Task<List<PrivateMessageDTO>> GetMessageHistoryAsync(string receiverUserId, int count)
        {
            var userId = Context.UserIdentifier;

            var messages = await _messageRepo.FindWithIncludesAsync(
                m =>
                    (m.SenderUserId == userId && m.ReceiverUserId == receiverUserId) ||
                    (m.SenderUserId == receiverUserId && m.ReceiverUserId == userId),
                m => m.SenderUser,
                m => m.ReceiverUser
            );

            return messages
                .OrderByDescending(m => m.SentAt)   // latest first
                .Take(count)
                .Select(m => new PrivateMessageDTO
                {
                    Id = m.Id,
                    SenderUserId = m.SenderUserId,
                    ReceiverUserId = m.ReceiverUserId,
                    SenderName = m.SenderUser.UserName,
                    ReceiverName = m.ReceiverUser.UserName,
                    MessageText = m.MessageText,
                    SentAt = m.SentAt,
                    IsRead = m.IsRead
                })
                .OrderBy(m => m.SentAt) // 👈 now sorted oldest → newest for UI
                .ToList();
        }
        public async Task<List<PrivateMessageDTO>> GetMessageHistory(string receiverUserId, int count)
        {
            return await GetMessageHistoryAsync(receiverUserId, count);
        }

        public async Task DeleteMessage(string messageId)
        {
            var currentUserId = Context.UserIdentifier;

            var message = await _messageRepo.GetByIdAsync(messageId);
            if (message == null)
            {
                await Clients.Caller.SendAsync("Error", "Message not found");
                return;
            }

            if (message.SenderUserId != currentUserId)
            {
                await Clients.Caller.SendAsync("Error", "You can only delete your own messages");
                return;
            }

            _messageRepo.Remove(message);
            await _messageRepo.SaveChangesAsync();

            await Clients
                .Users(message.SenderUserId, message.ReceiverUserId)
                .SendAsync("MessageDeleted", messageId);
        }

        public async Task SendTypingIndicator(string receiverUserId)
        {
            var senderId = Context.UserIdentifier;
            await Clients.User(receiverUserId).SendAsync("UserTyping", senderId);
        }
        public override Task OnConnectedAsync()
        {
            _logger.LogInformation("User connected with Id: {UserId}", Context.UserIdentifier);
            return base.OnConnectedAsync();
        }
    }
}
