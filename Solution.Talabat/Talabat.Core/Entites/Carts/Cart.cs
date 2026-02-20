namespace Talabat.Core.Entites.Carts
{
    public class Cart : BaseModelRedis // prop cartId string
    {
        public List<CartItems> Items { get; set; }

        public int? DeleveryMethodId { get; set; }
        public decimal? DeleveryMethodCost { get; set; }

        public string? ClientSecret { get; set; }
        public string? PaymentIntentId { get; set; }
    }
}
