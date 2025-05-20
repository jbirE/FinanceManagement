/*
SECTION 4: NOTIFICATION JOB
This is the scheduled job that checks for and sends notifications
periodically. It uses Quartz.NET for scheduling.
*/

using Quartz;
using System.Threading.Tasks;

namespace FinanceManagement.SignalRjobs
{
    // This attribute prevents multiple instances of the job running simultaneously
    [DisallowConcurrentExecution]
    public class NotificationJob : IJob
    {
        private readonly NotificationProvider _provider;

        public NotificationJob(NotificationProvider provider)
        {
            _provider = provider;
        }

        // This method is called by Quartz on the defined schedule
        public async Task Execute(IJobExecutionContext context)
        {
            // Simply call the provider to check and send notifications
            await _provider.CheckAndSendNotificationsAsync();
        }
    }
}
