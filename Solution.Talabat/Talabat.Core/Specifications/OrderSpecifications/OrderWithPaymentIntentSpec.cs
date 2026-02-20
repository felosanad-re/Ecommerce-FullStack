using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entites.Orders;

namespace Talabat.Core.Specifications.OrderSpecifications
{
    public class OrderWithPaymentIntentSpec : BaseSpecification<Order>
    {
        public OrderWithPaymentIntentSpec(string paymentIntentId)
            : base(O => O.PaymentId == paymentIntentId)
        {
            
        }
    }
}
