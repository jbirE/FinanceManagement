using FinanceManagement.Data.Models;
using FinanceManagement.SignalRjobs.Hubs;
using FinanceTool.Repositories.Interface;
using Microsoft.AspNetCore.SignalR;

public class NotificationProvider
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationProvider(
        IHubContext<NotificationHub> hubContext,
        IUnitOfWork unitOfWork)
    {
        _hubContext = hubContext;
        _unitOfWork = unitOfWork;
    }

    // This method is called by the Quartz job to send all unread notifications
    public async Task CheckAndSendNotificationsAsync()
    {
        // Get all unread notifications from the database
        var notifications = await _unitOfWork.Notifications
            .FindAsync(n => !n.IsReaded);

        // Group notifications by recipient
        var groupedNotifications = notifications
            .GroupBy(n => n.DestinataireId)
            .ToDictionary(g => g.Key, g => g.ToList());

        // Send notifications to each user
        foreach (var userGroup in groupedNotifications)
        {
            string userId = userGroup.Key;
            List<Notification> userNotifications = userGroup.Value;

            if (string.IsNullOrEmpty(userId))
                continue;

            // Send each notification to the user
            foreach (var notification in userNotifications)
            {
                await _hubContext.Clients.User(userId)
                    .SendAsync("ReceiveNotification", notification);
            }

            // Also send the count of unread notifications
            await UpdateNotificationsCount(userId, userNotifications.Count);

           
        }
    }

    public async Task UpdateNotificationsCount(string userId , int count)
    {
        // Also send the count of unread notifications
        await _hubContext.Clients.User(userId)
            .SendAsync("ReceiveNotificationCount", count);
    }

    // This method is called by services to send a notification immediately
    public async Task SendNotificationAsync(Notification notification)
    {
        if (notification == null || string.IsNullOrEmpty(notification.DestinataireId))
            return;

        // Send the notification to the specific user
        await _hubContext.Clients.User(notification.DestinataireId)
            .SendAsync("ReceiveNotification", notification);

        // Also update their notification count
        var count = await _unitOfWork.Notifications
            .CountAsync(n => n.DestinataireId == notification.DestinataireId && !n.IsReaded);

        await _hubContext.Clients.User(notification.DestinataireId)
            .SendAsync("ReceiveNotificationCount", count);
    }
}
