using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entites.Orders;

namespace Talabat.Core.Services.Contract.OrderService
{
    public interface IOrderBuilder
    {
        IOrderBuilder SetEmail(string email);
        IOrderBuilder SetDelivary(DelivaryMethod delivary);
        IOrderBuilder SetAddress(AddressShiper shiper);
        IOrderBuilder AddSupTotal(decimal supTotal);
        IOrderBuilder AddPayment(string paymentId);
        IOrderBuilder AddItems(ICollection<OrderItems> items);
        Order Build();
    }
}
