using Quartz;
namespace FinanceManagement.SignalRjobs.Notification
{
    public class NotificationJob : IJob
    {
        private readonly NotificationProvider _provider;

        public NotificationJob(NotificationProvider provider)
        {
            _provider = provider;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _provider.CheckAndSendNotificationsAsync();
        }
    }

}
