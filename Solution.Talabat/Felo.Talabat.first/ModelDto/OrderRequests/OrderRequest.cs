using Talabat.Core.Entites.Orders;

namespace Felo.Talabat.Api.ModelDto.OrderRequests
{
    public class OrderRequest
    {
        public AddressShiper addressShiper { get; set; }
        public int delivaryMethod { get; set; }
    }
}
