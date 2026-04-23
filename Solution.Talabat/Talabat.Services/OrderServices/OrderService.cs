using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Talabat.Core.Entites.Carts;
using Talabat.Core.Entites.Orders;
using Talabat.Core.Entites.Products;
using Talabat.Core.GenaricRepo;
using Talabat.Core.RequestModels.Import;
using Talabat.Core.RequestModels.Orders;
using Talabat.Core.ResponseModel.Import;
using Talabat.Core.Services.Contract.ImportServices;
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
        IMapper mapper,
        IimportService importService
        ) : IOrderServices
    {
        #region Services

        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IRedisRepo<Cart> _repoCart = repoCart;
        private readonly IOrderBuilder _orderBuilder = orderBuilder;
        private readonly IOrderTracingServiceHub _orderTracingHub = orderTracingHub;
        private readonly IMapper _mapper = mapper;
        private readonly IimportService _importService = importService;
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

        #region Get Order For Export
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
        #endregion

        #region  Get Orders ForImport Async
        public async Task<OrderImportResultDTO> GetOrdersForImportAsync(ImportDTO<OrderImportToReturnDTO> req)
        {
            // Read the same Excel file twice because each worksheet maps to a different DTO shape.
            var orderSheet = await _importService.ExcelImportAsync(new ImportDTO<OrderImportToReturnDTO>
            {
                File = req.File,
                Config = BuildImportConfig<OrderImportToReturnDTO>("Orders")
            });

            var orderItemsSheet = await _importService.ExcelImportAsync(new ImportDTO<OrderItemImportToReturnDTO>
            {
                File = req.File,
                Config = BuildImportConfig<OrderItemImportToReturnDTO>("OrderItems")
            });

            var errors = new List<string>();
            errors.AddRange(orderSheet.Errors);
            errors.AddRange(orderItemsSheet.Errors);

            var orderRows = orderSheet.Data;
            var itemRows = orderItemsSheet.Data;
            var result = new OrderImportResultDTO
            {
                TotalRows = orderRows.Count + itemRows.Count,
                Orders = orderRows,
                Items = itemRows
            };

            // Group all imported items by the worksheet OrderId so each order row can rebuild its snapshot.
            var groupedItems = itemRows
                .GroupBy(i => i.OrderId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var productLookup = (await _unitOfWork.RepositaryAsync<Product>().GetAllAsync())
                .ToDictionary(p => p.Id);
            var deliveryMethods = await _unitOfWork.RepositaryAsync<DelivaryMethod>().GetAllAsync();
            var deliveryMethodById = deliveryMethods.ToDictionary(d => d.Id);
            var deliveryMethodByName = deliveryMethods
                .GroupBy(d => d.ShortName, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            var existingIds = orderRows.Where(o => o.Id > 0).Select(o => o.Id).Distinct().ToList();
            var existingOrders = existingIds.Any()
                ? await _unitOfWork.RepositaryAsync<Order>().GetAllAsyncSpec(new OrderWithItemsSpec(existingIds))
                : new List<Order>();
            var existingOrdersById = existingOrders.ToDictionary(o => o.Id);

            var newOrders = new List<(Order Entity, OrderImportToReturnDTO SourceRow)>();

            foreach (var orderRow in orderRows)
            {
                if (string.IsNullOrWhiteSpace(orderRow.BuyerEmail))
                {
                    errors.Add($"Order row {orderRow.Id}: BuyerEmail is required.");
                    continue;
                }

                if (!groupedItems.TryGetValue(orderRow.Id, out var orderItems) || orderItems.Count == 0)
                {
                    errors.Add($"Order row {orderRow.Id}: no items were found in sheet 'OrderItems'.");
                    continue;
                }

                if (!TryResolveDeliveryMethod(orderRow, deliveryMethodById, deliveryMethodByName, out var deliveryMethod))
                {
                    errors.Add($"Order row {orderRow.Id}: delivery method is invalid.");
                    continue;
                }

                if (!TryParseOrderStatus(orderRow.OrderStatus, out var orderStatus))
                {
                    errors.Add($"Order row {orderRow.Id}: order status '{orderRow.OrderStatus}' is invalid.");
                    continue;
                }

                var builtItems = BuildOrderItems(orderRow.Id, orderItems, productLookup, errors);
                if (builtItems.Count == 0)
                {
                    errors.Add($"Order row {orderRow.Id}: all items are invalid.");
                    continue;
                }

                var subTotal = builtItems.Sum(i => i.Price * i.Count);
                var address = BuildAddress(orderRow.AddressShiper);
                var parsedOrderDate = ParseOrderDate(orderRow.OrderDate);

                if (orderRow.Id > 0 && existingOrdersById.TryGetValue(orderRow.Id, out var existingOrder))
                {
                    // Update the tracked order and replace its items with the imported snapshot.
                    existingOrder.BuyerEmail = orderRow.BuyerEmail.Trim();
                    existingOrder.OrderStatus = orderStatus;
                    existingOrder.DelivaryMethodId = deliveryMethod.Id;
                    existingOrder.DelivaryMethod = deliveryMethod;
                    existingOrder.AddressShiper = address;
                    existingOrder.SubTotal = subTotal;
                    existingOrder.PaymentId = string.IsNullOrWhiteSpace(orderRow.PaymentId) ? null : orderRow.PaymentId.Trim();

                    if (parsedOrderDate.HasValue)
                    {
                        existingOrder.OrderDate = parsedOrderDate.Value;
                    }

                    existingOrder.Items.Clear();
                    foreach (var item in builtItems)
                    {
                        existingOrder.Items.Add(item);
                    }

                    _unitOfWork.RepositaryAsync<Order>().Update(existingOrder);
                    result.UpdatedCount++;
                }
                else
                {
                    // New rows become fresh orders even if the sheet uses a temporary Id for joining items.
                    var newOrder = new Order
                    {
                        BuyerEmail = orderRow.BuyerEmail.Trim(),
                        OrderStatus = orderStatus,
                        DelivaryMethodId = deliveryMethod.Id,
                        DelivaryMethod = deliveryMethod,
                        AddressShiper = address,
                        Items = builtItems,
                        SubTotal = subTotal,
                        OrderDate = parsedOrderDate ?? DateTimeOffset.UtcNow,
                        PaymentId = string.IsNullOrWhiteSpace(orderRow.PaymentId) ? null : orderRow.PaymentId.Trim()
                    };

                    newOrder.Id = 0;
                    newOrders.Add((newOrder, orderRow));
                }
            }

            if (newOrders.Any())
            {
                await _unitOfWork.RepositaryAsync<Order>().AddRangeAsync(newOrders.Select(x => x.Entity));
            }

            await _unitOfWork.CompleteAsync();

            // Copy generated database Ids back to the returned rows for newly inserted orders.
            foreach (var savedOrder in newOrders)
            {
                savedOrder.SourceRow.Id = savedOrder.Entity.Id;
            }

            result.AddedCount = newOrders.Count;
            result.Errors = errors;
            return result;
        }

        #region Helper Methods
        private static ImportExcelConfig<T> BuildImportConfig<T>(string sheetName)
        {
            return new ImportExcelConfig<T>
            {
                SheetName = sheetName,
                StartRow = 2,
                HasHeader = true
            };
        }

        // Parse the combined shipping address produced by the export sheet.
        private static AddressShiper BuildAddress(string? addressValue)
        {
            var parts = (addressValue ?? string.Empty)
                .Split(" - ", StringSplitOptions.TrimEntries)
                .Where(part => !string.IsNullOrWhiteSpace(part))
                .ToArray();

            return new AddressShiper
            {
                FirstName = parts.ElementAtOrDefault(0) ?? string.Empty,
                LastName = parts.ElementAtOrDefault(1) ?? string.Empty,
                City = parts.ElementAtOrDefault(2) ?? string.Empty,
                Street = parts.ElementAtOrDefault(3) ?? string.Empty
            };
        }

        // Rebuild the imported order items from the second worksheet.
        private static List<OrderItems> BuildOrderItems(
            int orderId,
            IEnumerable<OrderItemImportToReturnDTO> orderItems,
            Dictionary<int, Product> productLookup,
            List<string> errors)
        {
            var builtItems = new List<OrderItems>();

            foreach (var itemRow in orderItems)
            {
                if (itemRow.Count <= 0)
                {
                    errors.Add($"Order item row for order {orderId} and product {itemRow.ProductId}: Count must be greater than zero.");
                    continue;
                }

                if (itemRow.Price < 0)
                {
                    errors.Add($"Order item row for order {orderId} and product {itemRow.ProductId}: Price cannot be negative.");
                    continue;
                }

                productLookup.TryGetValue(itemRow.ProductId, out var product);
                var productSnapshot = new ProductInOrderItem(
                    itemRow.ProductId,
                    string.IsNullOrWhiteSpace(itemRow.ProductName) ? product?.Name ?? string.Empty : itemRow.ProductName.Trim(),
                    product?.PictureUrl ?? string.Empty);

                builtItems.Add(new OrderItems(productSnapshot, itemRow.Count, itemRow.Price));
            }

            return builtItems;
        }

        private static DateTimeOffset? ParseOrderDate(string? orderDate)
        {
            if (string.IsNullOrWhiteSpace(orderDate))
            {
                return null;
            }

            return DateTimeOffset.TryParse(orderDate, out var parsedDate) ? parsedDate : null;
        }

        private static bool TryResolveDeliveryMethod(
            OrderImportToReturnDTO orderRow,
            Dictionary<int, DelivaryMethod> deliveryMethodById,
            Dictionary<string, DelivaryMethod> deliveryMethodByName,
            out DelivaryMethod deliveryMethod)
        {
            if (orderRow.DelivaryMethodId.HasValue &&
                deliveryMethodById.TryGetValue(orderRow.DelivaryMethodId.Value, out deliveryMethod!))
            {
                return true;
            }

            if (!string.IsNullOrWhiteSpace(orderRow.DelivaryMethodName) &&
                deliveryMethodByName.TryGetValue(orderRow.DelivaryMethodName.Trim(), out deliveryMethod!))
            {
                return true;
            }

            deliveryMethod = null!;
            return false;
        }

        private static bool TryParseOrderStatus(string? orderStatusValue, out OrderStatus orderStatus)
        {
            if (string.IsNullOrWhiteSpace(orderStatusValue))
            {
                orderStatus = default;
                return false;
            }

            if (Enum.TryParse(orderStatusValue.Trim(), true, out orderStatus))
            {
                return true;
            }

            foreach (var field in typeof(OrderStatus).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var enumMember = field.GetCustomAttribute<System.Runtime.Serialization.EnumMemberAttribute>();
                if (enumMember?.Value?.Equals(orderStatusValue.Trim(), StringComparison.OrdinalIgnoreCase) == true)
                {
                    orderStatus = (OrderStatus)field.GetValue(null)!;
                    return true;
                }
            }

            return false;
        }
        #endregion
        #endregion
    }
}
