using Talabat.Core.Entites.Orders;

namespace Felo.Talabat.Api.ModelDto.OrderRequests
{
    public class OrderToReturnDto
    {
        public int Id { get; set; }
        public string OrderStatus { get; set; }
        public int DelivaryMethodId { get; set; }
        public string DelivaryMethod { get; set; }
        public decimal DelivaryMethodCost { get; set; }
        public AddressShiper AddressShiper { get; set; }
        public ICollection<OrderItemsDto> Items { get; set; } = new HashSet<OrderItemsDto>();
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public string PaymentId { get; set; } = "";
    }
}
