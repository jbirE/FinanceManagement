namespace FinanceManagement.SignalRjobs.Hubs
{
    public interface INotificationClient
    {
        Task ReceiveNotification(Notification notification);
        Task ReceiveNotificationCount(int count);
    }
}
