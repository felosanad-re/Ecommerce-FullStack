namespace Talabat.Core.RequestModels
{
    public class AddProductRequest
    {
        public string Name { get; set; }
        public string Descripaion { get; set; }
        public string PictureUrl { get; set; }
        public decimal Price { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public int Stock { get; set; }
    }
}
