using System.ComponentModel.DataAnnotations;
using Talabat.Core.Entites.Orders;

namespace Felo.Talabat.Api.ModelDto.AdminModels
{
    public class UpdateOrderStatus
    {
        [Required(ErrorMessage ="Order Id Is Required")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Order Status Is Required")]
        public OrderStatus Status { get; set; }
    }
}
