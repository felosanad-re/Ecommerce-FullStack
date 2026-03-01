using Talabat.Core.Entites.SignalR;

namespace Talabat.Core.Specifications.NotificationsSpec
{
    public class NotificationSpec : BaseSpecification<Notifications>
    {
        public NotificationSpec(NotificationsParams notifications)
            :base
            (N =>
                (!notifications.IsRead.HasValue || N.IsRead == notifications.IsRead.Value)
             &&
                (!notifications.IsDeleted.HasValue || N.IsDeleted == notifications.IsDeleted.Value)
            )
        {
            AddPagination((notifications.PageIndex - 1) * notifications.PageSize, notifications.PageSize);
        }
    }
}
