using Microsoft.AspNetCore.SignalR;

namespace FinanceManagement.SignalRjobs.Hubs
{
    public class NotificationHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            return base.OnConnectedAsync();
        }
    }

}
