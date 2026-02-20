using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entites.Orders;

namespace Talabat.Core.Specifications.OrderSpecifications
{
    public class OrderWithItem : BaseSpecification<Order>
    {
        public OrderWithItem(string buyerEmail)
            : base(O => O.BuyerEmail == buyerEmail)
        {
            Includes.Add(P => P.Items);
            Includes.Add(P => P.DelivaryMethod!);

            AddOrderBy(P => P.OrderDate);
        }
        public OrderWithItem(int orderId, string buyerEmail)
            :base(O => O.BuyerEmail == buyerEmail && O.Id == orderId)
        {
            Includes.Add(P => P.Items);
            Includes.Add(P => P.DelivaryMethod!);
        }
    }
}
