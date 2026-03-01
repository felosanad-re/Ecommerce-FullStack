using AutoMapper;
using Felo.Talabat.Api.ModelDto.AdminModels;
using Felo.Talabat.Api.ModelDto.OrderRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Talabat.Core;
using Talabat.Core.Entites.SignalR;
using Talabat.Core.Services.Contract.NotificationsServices;
using Talabat.Core.Services.Contract.OrderService;
using Talabat.Core.Specifications.NotificationsSpec;
using Talabat.Core.Specifications.OrderSpecifications;
using Talabat.Core.Specifications.SpecModel;

namespace Felo.Talabat.Api.Controllers.Admin
{
    [Authorize]
    //[Authorize(Roles = SD.SUPER_ADMIN + "," + SD.ADMIN)]
    public class AdminOrderController : BaseController
    {
        #region Services
        private readonly IOrderServices _orderService;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public AdminOrderController(IOrderServices orderServices, IMapper mapper, INotificationService notificationService)
        {
            _orderService = orderServices;
            _mapper = mapper;
            _notificationService = notificationService;
        }
        #endregion

        #region GetAllOrders
        [Authorize(Roles = SD.SUPER_ADMIN + "," + SD.ADMIN)]
        [HttpGet("Orders")] // Get: /api/AdminOrder/Orders
        public async Task<ActionResult<Pagination<IReadOnlyList<OrderToReturnDto>>>> GetAllOrders([FromQuery]OrderParams @params)
        {
            var orders = await _orderService.GetOrdersAsync(@params);
            var data = _mapper.Map<IReadOnlyList<OrderToReturnDto>>(orders);
            return Ok(new Pagination<OrderToReturnDto>(@params.PageSize, @params.PageIndex, data, data.Count));
        }
        #endregion

        #region Update OrderStatus
        [Authorize(Roles = SD.SUPER_ADMIN + "," + SD.ADMIN)]
        [HttpPost("UpdateOrderStatus")] // Post: /api/AdminOrder/UpdateOrderStatus
        public async Task<ActionResult<OrderStatusToReturnDto>> UpdateStatus([FromBody] UpdateOrderStatus request)
        {
            var order = await _orderService.UpdateOrderStatusAsync(request.Id, request.Status);
            return Ok(_mapper.Map<OrderStatusToReturnDto>(order));
        }
        #endregion

        #region Get All Notification
        [HttpGet("GetAllNotification")] // Get: /api/AdminOrder/GetAllNotification
        public  async Task<ActionResult<Pagination<IReadOnlyList<Notifications>>>> GetAll([FromQuery]NotificationsParams @params)
        {
            var data = await _notificationService.GetAllAsync(@params);
            return Ok(new Pagination<Notifications>(@params.PageSize, @params.PageIndex, data, data.Count));
        }
        #endregion

        #region Get UnRead Notifications
        [HttpGet("GetUnReadNotification")] // Get: /api/AdminOrder/GetUnReadNotification
        public async Task<ActionResult<int>> GetUnreadNotification([FromQuery]NotificationsParams @params)
        {
            var count = await _notificationService.GetUnreadNotificationAsync(@params);
            return Ok(new
            {
                UnreadCount = $"The Total Unread Notifications Is: {count}",
            });
        }
        #endregion

        #region Edit Notification
        [HttpPut("EditNotification")] // Put: /api/AdminOrder/EditNotification
        public async Task<ActionResult<Notifications?>> EditNotification(int id)
             => await _notificationService.ReadNotificationsAsync(id);
        #endregion

        #region DeleteNotification
        [HttpDelete("DeleteNotification")] // Delete: /api/AdminOrder/DeleteNotification
        public async Task<ActionResult> DeleteNotification(int id)
        {
            await _notificationService.DeleteAsync(id);
            return Ok(new
            {
                Message = "Notification Deleted Succeeded"
            });
        }
        #endregion
    }
}
