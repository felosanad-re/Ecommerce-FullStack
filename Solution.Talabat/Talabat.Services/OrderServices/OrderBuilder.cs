using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entites.Carts;
using Talabat.Core.Entites.Orders;
using Talabat.Core.Services.Contract.OrderService;

namespace Talabat.Services.OrderServices
{
    public class OrderBuilder : IOrderBuilder
    {
        private readonly Order _order = new Order();

        public IOrderBuilder SetEmail(string email)
        {
             _order.BuyerEmail = email;
            return this;
        }

        public IOrderBuilder SetAddress(AddressShiper shiper)
        {
            _order.AddressShiper = shiper;
            return this;
        }

        public IOrderBuilder AddItems(ICollection<OrderItems> items)
        {
            _order.Items = items;
            return this;
        }

        public IOrderBuilder AddSupTotal(decimal supTotal) // Total Items Price
        {
            _order.SubTotal = supTotal;
            return this;
        }

        public IOrderBuilder SetDelivary(DelivaryMethod delivary)
        {
            _order.DelivaryMethod = delivary;
            _order.DelivaryMethodId = delivary.Id;
            return this;
        }

        public IOrderBuilder AddPayment(string paymentId)
        {
            _order.PaymentId = paymentId;
            return this;
        }

        public Order Build()
        {
            return _order;
        }


    }
}
