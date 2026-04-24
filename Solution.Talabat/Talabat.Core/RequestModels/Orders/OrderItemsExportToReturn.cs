using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entites.Orders;

namespace Talabat.Core.RequestModels.Orders
{
    public class OrderItemsExportToReturn
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public string? PictureUrl { get; set; }
        public int OrderId { get; set; }
    }
}
