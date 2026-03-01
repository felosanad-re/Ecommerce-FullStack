using Talabat.Core.Entites.Orders;

namespace Talabat.Core.Services.Contract.HubServices
{
    public interface IOrderTracingServiceHub
    {
        Task BroadcastOrderStatusChanges(int id, OrderStatus status);
    }
}
