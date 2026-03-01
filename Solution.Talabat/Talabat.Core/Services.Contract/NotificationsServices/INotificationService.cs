using Talabat.Core.Entites.SignalR;
using Talabat.Core.Specifications.NotificationsSpec;

namespace Talabat.Core.Services.Contract.NotificationsServices
{
    public interface INotificationService
    {
        Task<IReadOnlyList<Notifications>> GetAllAsync(NotificationsParams @params);

        Task<int> GetUnreadNotificationAsync(NotificationsParams @params);

        Task<Notifications?> ReadNotificationsAsync(int id);

        ValueTask DeleteAsync(int id);
    }
}
