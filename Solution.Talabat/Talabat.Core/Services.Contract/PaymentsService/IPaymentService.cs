using Talabat.Core.Entites.Carts;
using Talabat.Core.Entites.Orders;

namespace Talabat.Core.Services.Contract.PaymentsService
{
    public interface IPaymentService
    {
        // Service Create and update paymentIntent after confirm Order
        Task<Cart?> CreateAndUpdatePaymentIntent(string cartId);

        // Servce To Update PaymentIntent To Succeeded Or Faild get value after pay
        Task<Order> UpdatePaymentIntent(string paymentIntentId, bool IsSucceeded);

        Task<Order> UpdateOrderState(int orderId, bool IsSucceeded);
    }
}
