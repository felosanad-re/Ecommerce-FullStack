using Talabat.Core.Entites.SignalR;
using Talabat.Core.Services.Contract.NotificationsServices;
using Talabat.Core.Specifications.NotificationsSpec;
using Talabat.Core.UnitOfWork;

namespace Talabat.Services.NotificationServices
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<Notifications>> GetAllAsync(NotificationsParams @params)
        {
            //where(N => N.IsDelete == false)
            var spec = new NotificationSpec(@params);
            var notifications = await _unitOfWork.RepositaryAsync<Notifications>().GetAllAsyncSpec(spec);
            return notifications;
        }

        public async Task<Notifications?> ReadNotificationsAsync(int id)
        {
            var notification = await _unitOfWork.RepositaryAsync<Notifications>().Get(id);
            if (notification == null) return null;
            notification.Id = id;
            notification.IsRead = true;
            await _unitOfWork.CompleteAsync();

            return notification;
        }

        public async Task<int> GetUnreadNotificationAsync(NotificationsParams @params)
        {
            var spec = new NotificationSpec(@params);
            var notificationUnread = await _unitOfWork.RepositaryAsync<Notifications>().GetAllAsyncSpec(spec);
            return notificationUnread.Count;
        }

        public async ValueTask DeleteAsync(int id)
        {
            var notification = await _unitOfWork.RepositaryAsync<Notifications>().Get(id);
            _unitOfWork.RepositaryAsync<Notifications>().delete(notification!);
            await _unitOfWork.CompleteAsync();
        }
    }
}
