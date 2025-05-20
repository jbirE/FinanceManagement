using FinanceManagement.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceManagement.Repositories.Interface
{
    public interface INotificationRepository
    {
        Task<Notification> GetByIdAsync(int id);
        Task<IEnumerable<Notification>> GetByUserIdAsync(string userId);
        Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(string userId);
        Task AddAsync(Notification notification);
        Task MarkAsReadAsync(int notificationId);
        Task MarkAllAsReadAsync(string userId);
        Task DeleteAsync(int notificationId);
    }
}