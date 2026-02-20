using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entites.Orders
{
    public class Order : ModelBase
    {
        public Order()
        {
            
        }

        public Order(string buyerEmail, OrderStatus orderStatus, DelivaryMethod? delivaryMethod, AddressShiper addressShiper, int? delivaryMethodId, decimal subTotal, DateTimeOffset orderDate, string paymentId)
        {
            BuyerEmail = buyerEmail;
            OrderStatus = orderStatus;
            DelivaryMethod = delivaryMethod;
            AddressShiper = addressShiper;
            DelivaryMethodId = delivaryMethodId;
            SubTotal = subTotal;
            OrderDate = orderDate;
            PaymentId = paymentId;
        }

        public Order(string buyerEmail, DelivaryMethod? delivaryMethod, AddressShiper addressShiper, ICollection<OrderItems> items, decimal subTotal, string paymentId)
        {
            BuyerEmail = buyerEmail;
            DelivaryMethod = delivaryMethod;
            AddressShiper = addressShiper;
            Items = items;
            SubTotal = subTotal;
            PaymentId = paymentId;
        }

        public string BuyerEmail { get; set; }
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
        public DelivaryMethod? DelivaryMethod { get; set; } // NFP [one]
        public int? DelivaryMethodId { get; set; }
        public AddressShiper AddressShiper { get; set; }
        public ICollection<OrderItems> Items { get; set; } = new HashSet<OrderItems>(); // NFP[Many]
        public decimal SubTotal { get; set; } // item price * item count
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;
        public string? PaymentId { get; set; }

        public decimal GetTotal()
            => SubTotal + DelivaryMethod!.Cost;
    }
}
