using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace TechpertsSolutions
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            // Ensure JWT includes "sub" or "id"
            return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
