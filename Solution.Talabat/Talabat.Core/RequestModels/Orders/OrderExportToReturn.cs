using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entites.Orders;

namespace Talabat.Core.RequestModels.Orders
{
    public class OrderExportToReturn
    {
        public int Id { get; set; }
        public string BuyerEmail { get; set; }
        public string OrderStatus { get; set; }
        public string DelivaryMethodName { get; set; } // NFP [one]
        public int? DelivaryMethodId { get; set; }
        public string AddressShiper { get; set; }
        public decimal SubTotal { get; set; } // item price * item count
        public string OrderDate { get; set; }
        public string? PaymentId { get; set; }
    }
}
