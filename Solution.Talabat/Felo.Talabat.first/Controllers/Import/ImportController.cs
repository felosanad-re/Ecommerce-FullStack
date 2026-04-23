using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Talabat.Core.RequestModels.Import;
using Talabat.Core.ResponseModel.Import;
using Talabat.Core.Services.Contract.OrderService;
using Talabat.Core.Services.Contract.ProductServices;

namespace Felo.Talabat.Api.Controllers.Import
{
    [Authorize]
    public class ImportController : BaseController
    {
        private readonly IProductService _productService;
        private readonly IOrderServices _orderService;

        public ImportController(IProductService productService, IOrderServices orderService)
        {
            _productService = productService;
            _orderService = orderService;
        }

        [HttpPost("Products")] // Post: /api/Import/Products
        public async Task<ActionResult<ImportToReturnDTO<ProductImportToReturnDTO>>> ImportProducts([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { Message = "Excel file is required." });
            }

            var result = await _productService.GetProductsForImportAsync(new ImportDTO<ProductImportToReturnDTO>
            {
                File = file
            });

            return Ok(result);
        }

        [HttpPost("Orders")] // Post: /api/Import/Orders
        public async Task<ActionResult<OrderImportResultDTO>> ImportOrders([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { Message = "Excel file is required." });
            }

            var result = await _orderService.GetOrdersForImportAsync(new ImportDTO<OrderImportToReturnDTO>
            {
                File = file
            });

            return Ok(result);
        }
    }
}
