using Talabat.Core.Entites.Brands;
using Talabat.Core.Entites.Categories;
using Talabat.Core.Entites.StockTransactions;

namespace Talabat.Core.Entites.Products
{
    public class Product : ModelBase
    {
        public string Name { get; set; }
        public string Descripaion { get; set; }
        public string PictureUrl { get; set; }
        public decimal Price { get; set; }

        // NFP [One]--> Brand
        public int? BrandId { get; set; }
        public Brand? Brand { get; set; }

        // NFP [One]--> Category
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public int Stock { get; set; } // Product in Stock

        public ICollection<StockTransaction> StockTransactions { get; set; } = new HashSet<StockTransaction>();
    }
}
