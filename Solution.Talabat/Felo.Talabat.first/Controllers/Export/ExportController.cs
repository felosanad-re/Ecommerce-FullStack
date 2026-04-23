using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.Core.RequestModels.Exports;
using Talabat.Core.RequestModels.Orders;
using Talabat.Core.RequestModels.Products;
using Talabat.Core.Services.Contract.ExportServices;
using Talabat.Core.Services.Contract.OrderService;
using Talabat.Core.Services.Contract.ProductServices;

namespace Felo.Talabat.Api.Controllers.Export
{
    [Authorize]
    public class ExportController : BaseController
    {
        protected readonly IExportService _exportService;
        protected readonly IProductService _productService;
        protected readonly IOrderServices _orderServices;
        public ExportController(
            IExportService exportService,
            IProductService productService,
            IOrderServices orderServices)
        {
            _exportService = exportService;
            _productService = productService;
            _orderServices = orderServices;
        }

        #region Export Products
        [HttpGet("Products")] // Get: /api/Export/Products
        public async Task<IActionResult> ExportProducts()
        {
            var requests = new IExportRequest[]
            {
                new ExportsRequest<ProductExportToReturn>
                {
                    WorksheetName = "Products",
                    DataFetcher = () => _productService.GetProductForExport()
                }
            };

            var file = await _exportService.ExportAsync(requests);
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "Products.xlsx");
        }
        #endregion

        #region Export Orders
        [HttpGet("Orders")] // Get: /api/Export/Orders
        public async Task<IActionResult> ExportOrders()
        {
            var requests = new IExportRequest[]
            {
                new ExportsRequest<OrderExportToReturn>
                {
                    WorksheetName = "Orders",
                    DataFetcher = () => _orderServices.GetOrderForExport()
                },
                new ExportsRequest<OrderItemsExportToReturn>
                {
                    WorksheetName = "OrderItems",
                    DataFetcher = () => _orderServices.GetOrderItemsToExport()
                }
            };

            var file = await _exportService.ExportAsync(requests);
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "Orders.xlsx");
        }
        #endregion
    }
}
