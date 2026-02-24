using AutoMapper;
using Felo.Talabat.Api.ModelDto.OrderRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Talabat.Core;
using Talabat.Core.Entites.Orders;
using Talabat.Core.Services.Contract.OrderService;

namespace Felo.Talabat.Api.Controllers.Admin
{
    [Authorize]
    [Authorize(Roles = SD.SUPER_ADMIN + "," + SD.ADMIN)]
    public class AdminOrderController : BaseController
    {
        private readonly IOrderServices _orderService;
        private readonly IMapper _mapper;

        public AdminOrderController(IOrderServices orderServices, IMapper mapper)
        {
            _orderService = orderServices;
            _mapper = mapper;
        }

        #region GetAllOrders

        [HttpGet("Orders")] // Get: /api/AdminOrder/Orders
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetAllOrders()
        {
            var orders = await _orderService.GetOrdersAsync();
            return Ok(orders);
        }

        #endregion
    }
}
