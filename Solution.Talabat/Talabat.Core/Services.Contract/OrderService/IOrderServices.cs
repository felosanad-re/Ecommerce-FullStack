using Talabat.Core.Entites.Orders;

namespace Talabat.Core.Services.Contract.OrderService
{
    public interface IOrderServices
    {
        Task<Order?> CreateOrder(string cartId, string buyerEmail, AddressShiper addressShiper, int delivary);

        // Get All Items In Cart
        Task<IReadOnlyList<Order>> GetOrders(string buyerEmail);

        // Get One Item In cart
        Task<Order?> GetOrder(int orderId, string buyerEmail);

        Task DeleteOrder(string cartId, int orderId);

        Task<IReadOnlyList<DelivaryMethod>> GetDelivaryMethods();
    }
}
