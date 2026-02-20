namespace Talabat.Core.Entites.Carts
{
    public class CartItems
    {
        public int Id { get; set; } // ProductId
        public string Name { get; set; }
        public string PictureUrl { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; } = 1;
    }
}
