using Talabat.Core.Entites.Carts;

namespace Felo.Talabat.Api.ModelDto.Cart
{
    public class CartItemDto
    {
        public CartItems CartItems { get; set; }
        public int? DeliveryMethodId { get; set; }
    }
}
