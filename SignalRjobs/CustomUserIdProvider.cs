using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace FinanceManagement.SignalRjobs
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            // This extracts the user ID from the authentication token
            // It's the same ID as DestinataireId in your Notification model
            return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
