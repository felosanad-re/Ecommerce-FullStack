using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Talabat.Core.Entites.Orders;
using Talabat.Core.Services.Contract.HubServices;
using Talabat.Repositaries.Data.Hubs;

namespace Talabat.Services.HubServices
{
    public class OrderServiceTracking : IOrderTracingServiceHub
    {
        private readonly IHubContext<HubSignalR> _hub;

        public OrderServiceTracking(IHubContext<HubSignalR> hub)
        {
            _hub = hub;
        }

        public async Task BroadcastOrderStatusChanges(int id, OrderStatus status)
        {
            await _hub.Clients.Group($"order-{id}")
                .SendAsync("OrderStatusTracking", id, status);

            await _hub.Clients.Group("Admins")
                .SendAsync("OrderStatusTracking", id, status);
        }
    }
}
