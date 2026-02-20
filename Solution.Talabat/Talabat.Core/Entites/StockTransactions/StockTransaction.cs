using Talabat.Core.Entites.Products;

namespace Talabat.Core.Entites.StockTransactions
{
    public class StockTransaction : ModelBase
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public StockTransactionType Type { get; set; }

        public int Count { get; set; }

        public DateTime DateTime { get; set; } = DateTime.UtcNow;
    }
}
