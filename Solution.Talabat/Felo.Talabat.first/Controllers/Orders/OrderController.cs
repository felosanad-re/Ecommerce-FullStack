using AutoMapper;
using Felo.Talabat.Api.ModelDto.OrderRequests;
using Felo.Talabat.Api.ModelDto.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
using Talabat.Core.Entites.Orders;
using Talabat.Core.Services.Contract.OrderService;

namespace Felo.Talabat.Api.Controllers.Orders
{
    [Authorize]
    public class OrderController(
        IOrderServices orderService,
        IMapper mapper,
        IConfiguration configuration) : BaseController
    {
        private readonly IOrderServices _orderService = orderService;
        private readonly IMapper _mapper = mapper;
        #region Create Order

        [HttpPost("CreateOrder")] // Post: /api/Order/CreateOrder
        public async Task<ActionResult<OrderToReturnDto>> CreateOrder([FromBody] OrderRequest orderRequest)
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            string cartId = AddCartId();
            var order = await _orderService.CreateOrder(cartId, buyerEmail, orderRequest.addressShiper, orderRequest.delivaryMethod);
            if (order is null)
            {
                return BadRequest(new
                {
                    Message = "Failed to create order - cart might be empty or invalid"
                });
            }
            return Ok(_mapper.Map<OrderToReturnDto>(order));
        }
        #endregion

        #region Get Orders Details
        [HttpGet("GetOrders")] // Get: /api/Order/GetOrders
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrders()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var orders = await _orderService.GetOrders(userEmail!);
            return Ok(_mapper.Map<IReadOnlyList<OrderToReturnDto>>(orders));
        }
        #endregion

        #region Get Order Details

        [HttpGet("{id}")] // Get: /api/order/id
        public async Task<ActionResult<Order>> GetOrder([FromRoute] int id)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var order = await _orderService.GetOrder(id, userEmail!);
            return Ok(_mapper.Map<OrderToReturnDto>(order));
        }
        #endregion

        #region Delete Order
        [HttpDelete("DeleteOrder/{orderId}")] // Delete: /api/Order/DeleteOrder/id
        public async Task<ActionResult> DeleteOrder(int orderId)
        {
            var cartId = AddCartId();
            await _orderService.DeleteOrder(cartId, orderId);
            return Ok(new
            {
                message = "Order Deleted Succeded"
            });
        }
        #endregion

        #region Get Delivery Method
        [AllowAnonymous]
        [HttpGet("DeliveryMethod")] // Get: /api/Order/DeliveryMethod
        public async Task<ActionResult<DelivaryMethod>> GetDelivery()
        {
            var delivery = await _orderService.GetDelivaryMethods();
            return Ok(delivery);
        }
        #endregion

        private string AddCartId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartId = $"Cart:{userId}";
            return cartId;
        }
    }
}
