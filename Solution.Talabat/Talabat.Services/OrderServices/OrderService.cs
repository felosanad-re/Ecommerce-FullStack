using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;
using Talabat.Core.Entites.Carts;
using Talabat.Core.Entites.Orders;
using Talabat.Core.Entites.Products;
using Talabat.Core.GenaricRepo;
using Talabat.Core.RequestModels.Orders;
using Talabat.Core.Services.Contract.HubServices;
using Talabat.Core.Services.Contract.OrderService;
using Talabat.Core.Specifications.OrderSpecifications;
using Talabat.Core.UnitOfWork;

namespace Talabat.Services.OrderServices
{
    public class OrderService(
        IUnitOfWork unitOfWork,
        IRedisRepo<Cart> repoCart,
        IOrderBuilder orderBuilder,
        IOrderTracingServiceHub orderTracingHub,
        IMapper mapper
        ) : IOrderServices
    {
        #region Services

        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IRedisRepo<Cart> _repoCart = repoCart;
        private readonly IOrderBuilder _orderBuilder = orderBuilder;
        private readonly IOrderTracingServiceHub _orderTracingHub = orderTracingHub;
        private readonly IMapper _mapper = mapper;
        #endregion

        public async Task<IReadOnlyList<Order>> GetOrdersAsync(OrderParams @params)
        {
            var spec = new OrderWithItemsSpec(@params);
            var order = await _unitOfWork.RepositaryAsync<Order>().GetAllAsyncSpec(spec);

            return order;
        }

        #region Create Order Service
        public async Task<Order?> CreateOrder(string cartId, string buyerEmail, AddressShiper addressShiper, int delivary)
        {
            // 1.Get Cart
            var cart = await _repoCart.GetCacheAsync(cartId);
            if (cart == null || !cart.Items.Any())
            {
                throw new ValidationException("Cart is empty or does not exist");
            }
            // 2.Check On Items in Product
            var orderItems = new List<OrderItems>();
            if (cart?.Items?.Count > 0)
            {
                foreach (var item in cart.Items)
                {
                    var product = await _unitOfWork.RepositaryAsync<Product>().Get(item.Id);
                    var productInOrderItems = new ProductInOrderItem(product.Id, product.Name, product.PictureUrl);
                    var orderItem = new OrderItems(productInOrderItems, item.Count, product.Price);
                    orderItems.Add(orderItem);
                }
            }
            // 3.Get Delivery Method
            var deliveryMethod = await _unitOfWork.RepositaryAsync<DelivaryMethod>().Get(delivary);
            // 4. Calculate SubTotal
            var subTotal = orderItems.Sum(o => o.Price * o.Count);

            //check on ordder have paymentIntent or no
            /// var orderRepo = _unitOfWork.RepositaryAsync<Order>();
            /// 
            /// var orderSpec = new OrderWithPaymentIntentSpec(cart.PaymentIntentId);
            /// var paymentIntentExist = await orderRepo.GetSpec(orderSpec);
            /// if (paymentIntentExist != null)
            /// {
            ///     // هيحذف الاوردر القديم وبعد ما يحزفه هيروح يشيل الامونت بتاعه 
            ///     orderRepo.delete(paymentIntentExist);
            ///     await _paymentService.CreateAndUpdatePaymentIntent(cartId);
            /// }
            
            // 5.Add Order
            var order = _orderBuilder.SetEmail(buyerEmail)
                .SetAddress(addressShiper)
                .AddItems(orderItems)
                .SetDelivary(deliveryMethod!)
                .AddSupTotal(subTotal)
                //.AddPayment(cart.PaymentIntentId) // For Element Stripe
                .Build();
            // 6.Save
            await _unitOfWork.RepositaryAsync<Order>().AddAsync(order);
            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0) return null;
            return order;
        }
        #endregion

        #region Get All Orders
        public async Task<IReadOnlyList<Order>> GetOrders(string buyerEmail)
        {
            var orderRepo = _unitOfWork.RepositaryAsync<Order>();
            var spec = new OrderWithItem(buyerEmail);
            var order = await orderRepo.GetAllAsyncSpec(spec);
            return order is null ? throw new ValidationException("Cart is empty or does not exist") : order;
        }
        #endregion

        #region Get Order
        public Task<Order?> GetOrder(int orderId, string buyerEmail)
        {
            var spec = new OrderWithItem(orderId, buyerEmail);
            var order = _unitOfWork.RepositaryAsync<Order>().GetSpec(spec);
            return order;
        }
        #endregion

        #region Get Delivaery
        public async Task<IReadOnlyList<DelivaryMethod>> GetDelivaryMethods()
        {
            var delivery = await _unitOfWork.RepositaryAsync<DelivaryMethod>().GetAllAsync();
            return delivery;
        }
        #endregion

        #region Delete Order
        public async Task DeleteOrder(string cartId, int orderId)
        {
            var cart = await _repoCart.GetCacheAsync(cartId);
            /// if (cart?.Items?.Count > 0)
            /// {
            ///     foreach (var item in cart?.Items)
            ///     {
            ///         var product = await _unitOfWork.RepositaryAsync<Product>().Get(item.Id);
            ///         if (product != null)
            ///         {
            ///             product.IsAddedToCart = false;
            ///         }
            ///         _unitOfWork.RepositaryAsync<Product>().Update(product);
            ///     }
            ///     await _unitOfWork.CompleteAsync();
            /// }
            await _repoCart.RemoveCacheAsync(cartId);
            var order = await _unitOfWork.RepositaryAsync<Order>().Get(orderId);
            _unitOfWork.RepositaryAsync<Order>().delete(order!);
            await _unitOfWork.CompleteAsync();
        }
        #endregion

        #region Tracking Order Status
        // For Admin 
        public async Task<Order?> UpdateOrderStatusAsync(int id, OrderStatus status)
        {
            var spec = new OrderWithItemsSpec(id);
            var updateOrderStatus = await _unitOfWork.RepositaryAsync<Order>().GetSpec(spec);
            if (updateOrderStatus is null) throw new Exception("Order Not Found");

            updateOrderStatus.OrderStatus = status;
            await _unitOfWork.CompleteAsync();

            // SignalR
            await _orderTracingHub.BroadcastOrderStatusChanges(id, status);
            return updateOrderStatus;
        }

        #endregion
        public async Task<IReadOnlyList<OrderExportToReturn>> GetOrderForExport()
        {
            var data = await _unitOfWork.RepositaryAsync<Order>().GetAllAsyncSpec(new OrderWithItemsSpec());
            var result = _mapper.Map<IReadOnlyList<OrderExportToReturn>>(data);

            // Get Shipping Address
            foreach (var order in result)
            {
                // Get Address for each order
                var address = data.FirstOrDefault(o => o.Id == order.Id)?.AddressShiper;
                if(address != null)
                {
                    var parts = new[]
                    {
                        address.FirstName,
                        address.LastName,
                        address.City,
                        address.Street
                    };
                    order.AddressShiper = string.Join(" - ", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
                }
            }
            return result;
        }

        public async Task<IReadOnlyList<OrderItemsExportToReturn>> GetOrderItemsToExport()
        {
            var orders = await _unitOfWork.RepositaryAsync<Order>().GetAllAsyncSpec(new OrderWithItemsSpec());
            var result = orders.Where(o => o.Items != null && o.Items.Any())
                .SelectMany(o => o.Items, (order, item) => new OrderItemsExportToReturn
                {
                    OrderId = order.Id, // To Attach every item with his order Id
                    Count = item.Count,
                    Price = item.Price,
                    ProductId = item.Product.ProductId,
                    ProductName = item.Product.Name
                }).ToList();
            return result;
        }
    }
}
