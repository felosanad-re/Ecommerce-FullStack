using System.ComponentModel.DataAnnotations.Schema;
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

        public ICollection<StockTransaction> StockTransactions { get; set; } = new HashSet<StockTransaction>();


        private int stock;

        public int Stock
        {
            get =>  stock;
            set
            {
                stock = Math.Max(0, value);
            }
        }
        private readonly int LowStockThreshold = 5;

        public StockType StockType
        {
            get
            {
                if (Stock <= 0)
                    return StockType.OUTOFSTOCK;

                if (Stock <= LowStockThreshold)
                    return StockType.LOWSTOCK;

                return StockType.INSTOCK;
            }
        }
    }
}
