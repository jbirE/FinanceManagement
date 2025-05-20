using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
/*
SECTION 2: CUSTOM USER ID PROVIDER
This tells SignalR how to identify which user each connection belongs to.
Without this, SignalR wouldn't know which connection to send notifications to.
*/
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
