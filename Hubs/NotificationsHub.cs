using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Hubs
{ 
    public class NotificationsHub : Hub
    {
        private static readonly ConcurrentDictionary<string, HashSet<string>> _userConnections = new();

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                var connections = _userConnections.GetOrAdd(userId, _ => new HashSet<string>());
                lock (connections) connections.Add(Context.ConnectionId);

                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId) && _userConnections.TryGetValue(userId, out var connections))
            {
                lock (connections) connections.Remove(Context.ConnectionId);
                if (connections.Count == 0)
                    _userConnections.TryRemove(userId, out _);

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinUserGroup(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
        }

        public async Task LeaveUserGroup(string userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
        }

        public static List<string> GetConnectionIds(string userId)
        {
            return _userConnections.TryGetValue(userId, out var connections)
                ? connections.ToList()
                : new List<string>();
        }

        public static List<string> GetConnectionIds(List<string> userIds)
        {
            return userIds.SelectMany(id => GetConnectionIds(id)).ToList();
        }
    }
}
