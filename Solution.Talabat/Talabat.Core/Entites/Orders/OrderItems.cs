using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entites.Orders
{
    public class OrderItems : ModelBase
    {
        public OrderItems()
        {
            
        }

        public OrderItems(ProductInOrderItem product, int count, decimal price)
        {
            Product = product;
            Count = count;
            Price = price;
        }

        public ProductInOrderItem Product { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
    }
}
