namespace Felo.Talabat.Api.ModelDto.OrderRequests
{
    public class OrderItemsDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string PictureUrl { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
    }
}
