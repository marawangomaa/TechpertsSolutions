using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;


namespace Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatHub : Hub
    {
        private readonly IRepository<PrivateMessage> _messageRepo;
        private readonly ILogger<ChatHub> _logger;
        public ChatHub(ILogger<ChatHub> logger, IRepository<PrivateMessage> messageRepo)
        {
            _logger = logger;
            _messageRepo = messageRepo;
        }

        public async Task SendPrivateMessage(string receiverUserId, string message)
        {
            var senderId = Context.UserIdentifier; // must be set in authentication
            var senderRole = Context.User?.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

            // Save to DB
            var privateMessage = new PrivateMessage
            {
                SenderUserId = senderId,
                ReceiverUserId = receiverUserId,
                SenderRole = senderRole,
                MessageText = message,
                SentAt = DateTime.UtcNow
            };

            await _messageRepo.AddAsync(privateMessage);
            await _messageRepo.SaveChangesAsync();

            // Send only to the specific user
            await Clients.User(receiverUserId).SendAsync("ReceivePrivateMessage", senderId, message);
        }
    }
}
