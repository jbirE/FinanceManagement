namespace FinanceManagement.SignalRjobs.Hubs
{
    public interface IClientHub
    {
        Task ReceiveNotifications(object data);
    }
}
