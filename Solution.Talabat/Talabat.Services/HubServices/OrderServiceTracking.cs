using Microsoft.AspNetCore.SignalR;
using Talabat.Core.Entites.Orders;
using Talabat.Core.Entites.SignalR;
using Talabat.Core.Services.Contract.HubServices;
using Talabat.Core.UnitOfWork;
using Talabat.Repositaries.Data.Hubs;

namespace Talabat.Services.HubServices
{
    public class OrderServiceTracking : IOrderTracingServiceHub
    {
        private readonly IHubContext<HubSignalR> _hub;
        private readonly IUnitOfWork _unitOfWork;

        public OrderServiceTracking(IHubContext<HubSignalR> hub, IUnitOfWork unitOfWork)
        {
            _hub = hub;
            _unitOfWork = unitOfWork;
        }

        public async Task BroadcastOrderStatusChanges(int id, OrderStatus status)
        {
            var notification = new Notifications
            {
                OrderId = id, // OrderId
                Status = status.ToString(),
                Message = $"the order-{id}, has update status changes To: {status}"
            };
            // User
            await _hub.Clients.Group($"order-{id}")
                .SendAsync("OrderStatusTracking", notification);

            // Admin
            await _hub.Clients.Group("Admins")
                .SendAsync("OrderStatusTracking", notification);

            await _unitOfWork.RepositaryAsync<Notifications>().AddAsync(notification);
            await _unitOfWork.CompleteAsync();
        }
    }
}
