using Microsoft.Extensions.Configuration;
using Stripe;
using Talabat.Core.Entites.Carts;
using Talabat.Core.Entites.Orders;
using Talabat.Core.GenaricRepo;
using Talabat.Core.Services.Contract.PaymentsService;
using Talabat.Core.Specifications.OrderSpecifications;
using Talabat.Core.UnitOfWork;
using Product = Talabat.Core.Entites.Products.Product;

namespace Talabat.Services.PaymentServices
{
    public class PaymentService(IConfiguration configuration, IUnitOfWork unitOfWork, IRedisRepo<Cart> cartRepo) : IPaymentService
    {
        public async Task<Cart?> CreateAndUpdatePaymentIntent(string cartId)
        {
            // Set Secret Key
            StripeConfiguration.ApiKey = configuration["StripeSitting:SecretKey"];
            // Get delivary method cost
            var cart = await cartRepo.GetCacheAsync(cartId);
            if (cart is null) return null;

            //cart.DeleveryMethodId = deliveryMethodId;

            var delivaryMethodCost = 0m;
            if(cart.DeleveryMethodId.HasValue)
            {
                var delivary = await unitOfWork.RepositaryAsync<DelivaryMethod>().Get(cart.DeleveryMethodId);
                cart.DeleveryMethodCost = delivary!.Cost;
                delivaryMethodCost = delivary.Cost;
            }
            // get items in cart
            if (cart?.Items?.Count() > 0) {
                foreach (var item in cart?.Items)
                {
                    var product = await unitOfWork.RepositaryAsync<Product>().Get(item.Id);
                    if (item.Price != product.Price)
                        item.Price = product.Price;
                }
            }

            // Create PaymentIntent Service
            PaymentIntentService paymentIntentService = new PaymentIntentService();
            // Create payemntIntent
            PaymentIntent paymentIntent;

            // Check if cart has Paymentintent
            if (string.IsNullOrEmpty(cart.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions()
                {
                    Amount = (long)(cart.Items.Sum(item => item.Price * item.Count * 100)
                            + delivaryMethodCost * 100),
                    Currency = "USD",
                    PaymentMethodTypes = new List<string> { "card" }
                };
                paymentIntent = await paymentIntentService.CreateAsync(options);
                cart.PaymentIntentId = paymentIntent.Id;
                cart.ClientSecret = paymentIntent.ClientSecret;
            }
            else // update payment
            {
                var options = new PaymentIntentUpdateOptions()
                {
                    Amount = (long) (cart.Items.Sum(item => item.Price * item.Count * 100)
                            + delivaryMethodCost * 100)
                };
                await paymentIntentService.UpdateAsync(cart.PaymentIntentId, options);
            }
            await cartRepo.SetCacheAsync(cart);
            return cart;
        }

        public async Task<Order> UpdateOrderState(int orderId, bool IsSucceeded)
        {
            var order = await unitOfWork.RepositaryAsync<Order>().Get(orderId);
            // Set OrderStatus
            if (IsSucceeded)
                order.OrderStatus = OrderStatus.PaymentSuccedded;
            else
                order.OrderStatus = OrderStatus.PaymentFaild;
            // Save New Value
            unitOfWork.RepositaryAsync<Order>().Update(order);
            await unitOfWork.CompleteAsync();
            return order;
        }

        public async Task<Order> UpdatePaymentIntent(string paymentIntentId, bool IsSucceeded)
        {
            var spec = new PaymentIntentWithOrderSpec(paymentIntentId);
            var order = await unitOfWork.RepositaryAsync<Order>().GetSpec(spec);
            // Set OrderStatus
            if (IsSucceeded)
                order.OrderStatus = OrderStatus.PaymentSuccedded;
            else
                order.OrderStatus = OrderStatus.PaymentFaild;
           // Save New Value
            unitOfWork.RepositaryAsync<Order>().Update(order);
            await unitOfWork.CompleteAsync();
            return order;
        }
    }
}
