using Talabat.Core.Entites.Brands;
using Talabat.Core.Entites.Categories;

namespace Felo.Talabat.Api.ModelDto.Products
{
    public class ProductToReturnDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Descripaion { get; set; }
        public string PictureUrl { get; set; }
        public decimal Price { get; set; }
        public int BrandId { get; set; }
        public string Brand { get; set; }

        public int CategoryId { get; set; }
        public string Category { get; set; }
        public bool IsAddedToCart { get; set; }

        public int Stock { get; set; }
        public string IsDeleted { get; set; }
    }
}
