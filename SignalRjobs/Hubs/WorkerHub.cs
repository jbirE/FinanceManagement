using FinanceManagement.SignalRjobs.HubResults;
using Microsoft.AspNetCore.SignalR;

namespace FinanceManagement.SignalRjobs.Hubs
{
    public class WorkerHub : Hub<IClientHub>
    {
        private readonly IHubContext<ClientHub, IClientHub> _clientHubContext;
        public WorkerHub(IHubContext<ClientHub, IClientHub> clientContext) {
            _clientHubContext = clientContext;
        }

        public async Task SendNotifications(SignalRRescult result)
        {
            var client = _clientHubContext.Clients.Client(result.ConnectionId);
            if (client == null)
                return;

            await client.ReceiveNotifications(result);

        }
    }
}
