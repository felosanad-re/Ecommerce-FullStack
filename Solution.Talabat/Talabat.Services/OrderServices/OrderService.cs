using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;
using System.IO.Compression;
using System.Reflection;
using Talabat.Core.Entites.Carts;
using Talabat.Core.Entites.Orders;
using Talabat.Core.Entites.Products;
using Talabat.Core.GenaricRepo;
using Talabat.Core.RequestModels.Import;
using Talabat.Core.RequestModels.Orders;
using Talabat.Core.ResponseModel.Import;
using Talabat.Core.Services.Contract.AttachmentService;
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
        IimportService importService,
        IAttachmentService attachmentService
        ) : IOrderServices
    {
        #region Services

        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IRedisRepo<Cart> _repoCart = repoCart;
        private readonly IOrderBuilder _orderBuilder = orderBuilder;
        private readonly IOrderTracingServiceHub _orderTracingHub = orderTracingHub;
        private readonly IMapper _mapper = mapper;
        private readonly IimportService _importService = importService;
        private readonly IAttachmentService _attachmentService = attachmentService;
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
                    ProductName = item.Product.Name,
                    PictureUrl = item.Product.PictureUrl
                }).ToList();
            return result;
        }
        #endregion

        #region  Get Orders ForImport Async
        public async Task<OrderImportResultDTO> GetOrdersForImportAsync(ImportDTO<OrderImportToReturnDTO> req)
        {
            var (orderRows, itemRows, errors, result) = await LoadImportSheetsAsync(req);
            var skippedDuplicates = 0;

            // ── Extract images from the optional zip file and upload them ──
            // Key = file name without extension (matched by ProductId or ProductName), Value = uploaded URL
            var uploadedImages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (req.ZipFile is not null && req.ZipFile.Length > 0)
            {
                var imageEntries = ExtractImageEntries(req.ZipFile, errors);

                foreach (var (fileName, imageData) in imageEntries)
                {
                    try
                    {
                        var uploadedFileName = await UploadImageAsync(fileName, imageData);
                        // Store by file name without extension so we can match "1.png" → key "1", "iPhone.png" → key "iPhone"
                        var key = Path.GetFileNameWithoutExtension(fileName);
                        if (!string.IsNullOrWhiteSpace(key))
                        {
                            uploadedImages[key] = uploadedFileName;
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Failed to upload image '{fileName}': {ex.Message}");
                    }
                }
            }

            // Set PictureUrl on each item row from uploaded images or existing product data.
            foreach (var itemRow in itemRows)
            {
                // Try matching by ProductId first, then by ProductName
                if (uploadedImages.TryGetValue(itemRow.ProductId.ToString(), out var uploadedUrl))
                {
                    itemRow.PictureUrl = uploadedUrl;
                }
                else if (!string.IsNullOrWhiteSpace(itemRow.ProductName) &&
                         uploadedImages.TryGetValue(itemRow.ProductName.Trim(), out uploadedUrl))
                {
                    itemRow.PictureUrl = uploadedUrl;
                }
            }

            orderRows = NormalizeImportedOrders(orderRows, errors, ref skippedDuplicates);
            itemRows = NormalizeImportedOrderItems(itemRows, errors, ref skippedDuplicates);

            // Group all imported items by the worksheet OrderId so each order row can rebuild its snapshot.
            var groupedItems = itemRows
                .GroupBy(i => i.OrderId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var productRepo = _unitOfWork.RepositaryAsync<Product>();
            var productLookup = (await productRepo.GetAllAsync())
                .ToDictionary(p => p.Id);

            // ── Update Product.PictureUrl in the database for any matched uploaded images ──
            if (uploadedImages.Count > 0)
            {
                foreach (var product in productLookup.Values)
                {
                    if (uploadedImages.TryGetValue(product.Id.ToString(), out var urlById))
                    {
                        product.PictureUrl = urlById;
                        productRepo.Update(product);
                    }
                    else if (!string.IsNullOrWhiteSpace(product.Name) &&
                             uploadedImages.TryGetValue(product.Name.Trim(), out var urlByName))
                    {
                        product.PictureUrl = urlByName;
                        productRepo.Update(product);
                    }
                }
            }

            ApplyImportedPriceUpdates(itemRows, productLookup, productRepo);

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
                    if (!HasOrderChanges(existingOrder, orderRow, builtItems, deliveryMethod, orderStatus, address, subTotal, parsedOrderDate))
                    {
                        skippedDuplicates++;
                        continue;
                    }

                    ApplyImportedOrderValues(
                        existingOrder,
                        orderRow,
                        deliveryMethod,
                        orderStatus,
                        address,
                        subTotal,
                        parsedOrderDate);

                    MergeOrderItems(existingOrder, builtItems);

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
            result.SkippedDuplicates = skippedDuplicates;
            result.Errors = errors;
            return result;
        }

        #region Helper Methods
        private async Task<(List<OrderImportToReturnDTO> Orders, List<OrderItemImportToReturnDTO> Items, List<string> Errors, OrderImportResultDTO Result)>
            LoadImportSheetsAsync(ImportDTO<OrderImportToReturnDTO> req)
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

            var result = new OrderImportResultDTO
            {
                TotalRows = orderSheet.Data.Count,
                Orders = orderSheet.Data,
                Items = orderItemsSheet.Data
            };

            return (orderSheet.Data, orderItemsSheet.Data, errors, result);
        }

        private static List<OrderImportToReturnDTO> NormalizeImportedOrders(
            IEnumerable<OrderImportToReturnDTO> orderRows,
            List<string> errors,
            ref int skippedDuplicates)
        {
            var normalizedOrders = new List<OrderImportToReturnDTO>();
            var importedIds = new HashSet<int>();

            foreach (var orderRow in orderRows)
            {
                if (orderRow.Id > 0 && !importedIds.Add(orderRow.Id))
                {
                    errors.Add($"Duplicate order row detected for Id {orderRow.Id}. The last row in the file was used.");
                    skippedDuplicates++;
                    normalizedOrders.RemoveAll(o => o.Id == orderRow.Id);
                }

                normalizedOrders.Add(orderRow);
            }

            return normalizedOrders;
        }

        private static List<OrderItemImportToReturnDTO> NormalizeImportedOrderItems(
            IEnumerable<OrderItemImportToReturnDTO> itemRows,
            List<string> errors,
            ref int skippedDuplicates)
        {
            var normalizedItems = new Dictionary<(int OrderId, int ProductId), OrderItemImportToReturnDTO>();

            foreach (var itemRow in itemRows)
            {
                var key = (itemRow.OrderId, itemRow.ProductId);

                if (normalizedItems.TryGetValue(key, out var existingRow))
                {
                    var hasChanges =
                        existingRow.Count != itemRow.Count ||
                        existingRow.Price != itemRow.Price ||
                        !string.Equals(existingRow.ProductName?.Trim(), itemRow.ProductName?.Trim(), StringComparison.OrdinalIgnoreCase) ||
                        !string.Equals(existingRow.PictureUrl, itemRow.PictureUrl, StringComparison.OrdinalIgnoreCase);

                    if (hasChanges)
                    {
                        errors.Add($"Duplicate order item detected for order {itemRow.OrderId} and product {itemRow.ProductId}. The last row in the file was used.");
                    }

                    skippedDuplicates++;
                }

                normalizedItems[key] = itemRow;
            }

            return normalizedItems.Values.ToList();
        }

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

        private static void ApplyImportedOrderValues(
            Order existingOrder,
            OrderImportToReturnDTO orderRow,
            DelivaryMethod deliveryMethod,
            OrderStatus orderStatus,
            AddressShiper address,
            decimal subTotal,
            DateTimeOffset? parsedOrderDate)
        {
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
        }

        private static bool HasOrderChanges(
            Order existingOrder,
            OrderImportToReturnDTO orderRow,
            IReadOnlyCollection<OrderItems> importedItems,
            DelivaryMethod deliveryMethod,
            OrderStatus orderStatus,
            AddressShiper address,
            decimal subTotal,
            DateTimeOffset? parsedOrderDate)
        {
            if (!string.Equals(existingOrder.BuyerEmail, orderRow.BuyerEmail.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (existingOrder.OrderStatus != orderStatus ||
                existingOrder.DelivaryMethodId != deliveryMethod.Id ||
                existingOrder.SubTotal != subTotal ||
                !string.Equals(existingOrder.PaymentId ?? string.Empty, orderRow.PaymentId?.Trim() ?? string.Empty, StringComparison.Ordinal))
            {
                return true;
            }

            if (!AreAddressesEqual(existingOrder.AddressShiper, address))
            {
                return true;
            }

            if (parsedOrderDate.HasValue && existingOrder.OrderDate != parsedOrderDate.Value)
            {
                return true;
            }

            return !AreOrderItemsEqual(existingOrder.Items, importedItems);
        }

        private static bool AreAddressesEqual(AddressShiper current, AddressShiper imported)
        {
            return string.Equals(current.FirstName, imported.FirstName, StringComparison.OrdinalIgnoreCase)
                && string.Equals(current.LastName, imported.LastName, StringComparison.OrdinalIgnoreCase)
                && string.Equals(current.City, imported.City, StringComparison.OrdinalIgnoreCase)
                && string.Equals(current.Street, imported.Street, StringComparison.OrdinalIgnoreCase);
        }

        private static bool AreOrderItemsEqual(IEnumerable<OrderItems> currentItems, IEnumerable<OrderItems> importedItems)
        {
            var currentMap = currentItems
                .GroupBy(item => item.Product.ProductId)
                .ToDictionary(group => group.Key, group => group.First());

            var importedMap = importedItems
                .GroupBy(item => item.Product.ProductId)
                .ToDictionary(group => group.Key, group => group.Last());

            if (currentMap.Count != importedMap.Count)
            {
                return false;
            }

            foreach (var importedEntry in importedMap)
            {
                if (!currentMap.TryGetValue(importedEntry.Key, out var currentItem))
                {
                    return false;
                }

                var importedItem = importedEntry.Value;
                if (currentItem.Count != importedItem.Count ||
                    currentItem.Price != importedItem.Price ||
                    !string.Equals(currentItem.Product.Name, importedItem.Product.Name, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(currentItem.Product.PictureUrl, importedItem.Product.PictureUrl, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        private static void ApplyImportedPriceUpdates(
            IEnumerable<OrderItemImportToReturnDTO> itemRows,
            Dictionary<int, Product> productLookup,
            IGenaricRepo<Product> productRepo)
        {
            foreach (var latestItem in itemRows)
            {
                if (!productLookup.TryGetValue(latestItem.ProductId, out var product))
                {
                    continue;
                }

                if (product.Price == latestItem.Price)
                {
                    continue;
                }

                product.Price = latestItem.Price;
                productRepo.Update(product);
            }
        }

        private void MergeOrderItems(Order existingOrder, IEnumerable<OrderItems> importedItems)
        {
            var orderItemsRepo = _unitOfWork.RepositaryAsync<OrderItems>();
            var currentItemsByProductId = existingOrder.Items
                .GroupBy(item => item.Product.ProductId)
                .ToDictionary(group => group.Key, group => group.First());

            var importedItemsByProductId = importedItems
                .GroupBy(item => item.Product.ProductId)
                .ToDictionary(group => group.Key, group => group.Last());

            var itemsToRemove = existingOrder.Items
                .Where(item => !importedItemsByProductId.ContainsKey(item.Product.ProductId))
                .ToList();

            if (itemsToRemove.Count > 0)
            {
                orderItemsRepo.RemoveRange(itemsToRemove);

                foreach (var item in itemsToRemove)
                {
                    existingOrder.Items.Remove(item);
                }
            }

            foreach (var importedItem in importedItemsByProductId.Values)
            {
                if (currentItemsByProductId.TryGetValue(importedItem.Product.ProductId, out var currentItem))
                {
                    currentItem.Count = importedItem.Count;
                    currentItem.Price = importedItem.Price;
                    currentItem.Product.Name = importedItem.Product.Name;
                    currentItem.Product.PictureUrl = importedItem.Product.PictureUrl;
                    continue;
                }

                existingOrder.Items.Add(importedItem);
            }
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

                // Prefer the uploaded image from the zip (PictureUrl already set on the row),
                // otherwise fall back to the existing product image.
                var pictureUrl = !string.IsNullOrWhiteSpace(itemRow.PictureUrl)
                    ? itemRow.PictureUrl
                    : product?.PictureUrl ?? string.Empty;

                var productSnapshot = new ProductInOrderItem(
                    itemRow.ProductId,
                    string.IsNullOrWhiteSpace(itemRow.ProductName) ? product?.Name ?? string.Empty : itemRow.ProductName.Trim(),
                    pictureUrl);

                builtItems.Add(new OrderItems(productSnapshot, itemRow.Count, itemRow.Price));
            }

            return builtItems;
        }

        /// <summary>
        /// Extracts image entries from a zip file. Returns a list of (fileName, imageData) tuples.
        /// Only files with allowed image extensions are extracted; sub-folders are flattened.
        /// </summary>
        private static List<(string FileName, byte[] Data)> ExtractImageEntries(
            IFormFile zipFile,
            List<string> errors)
        {
            var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".png", ".jpg", ".jpeg" };
            var entries = new List<(string FileName, byte[] Data)>();

            try
            {
                using var zipStream = zipFile.OpenReadStream();
                using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read);

                foreach (var entry in archive.Entries)
                {
                    var extension = Path.GetExtension(entry.Name);
                    if (!allowedExtensions.Contains(extension))
                        continue;

                    // Skip directory entries
                    if (entry.Length == 0)
                        continue;

                    using var entryStream = entry.Open();
                    using var ms = new MemoryStream();
                    entryStream.CopyTo(ms);
                    entries.Add((entry.Name, ms.ToArray()));
                }
            }
            catch (InvalidDataException ex)
            {
                errors.Add($"The uploaded zip file is invalid or corrupted: {ex.Message}");
            }

            return entries;
        }

        /// <summary>
        /// Uploads a single image using the AttachmentService and returns the stored file name.
        /// </summary>
        private async Task<string> UploadImageAsync(string fileName, byte[] imageData)
        {
            // Create a temporary IFormFile from the byte array so the AttachmentService can process it.
            using var stream = new MemoryStream(imageData);
            var formFile = new FormFile(stream, 0, imageData.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = GetContentType(fileName)
            };

            var uploadedFileName = await _attachmentService.UploadAsync(formFile, "products");
            return uploadedFileName;
        }

        private static string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                _ => "application/octet-stream"
            };
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
