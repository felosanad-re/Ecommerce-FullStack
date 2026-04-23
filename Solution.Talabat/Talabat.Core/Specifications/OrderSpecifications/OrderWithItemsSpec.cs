using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entites.Orders;

namespace Talabat.Core.Specifications.OrderSpecifications
{
    public class OrderWithItemsSpec: BaseSpecification<Order>
    {
        public OrderWithItemsSpec()
            :base()
        {
            Includes.Add(O => O.Items);
            Includes.Add(O => O.DelivaryMethod!);
        }
        public OrderWithItemsSpec(OrderParams @params)
            :base()
        {
            Includes.Add(O => O.Items);
            Includes.Add(O => O.DelivaryMethod!);
            AddPagination((@params.PageIndex - 1) * @params.PageSize, @params.PageSize);
        }

        public OrderWithItemsSpec(int orderId)
            : base(O => O.Id == orderId)
        {
            Includes.Add(O => O.Items);
            Includes.Add(O => O.DelivaryMethod!);
        }

        public OrderWithItemsSpec(IEnumerable<int> orderIds)
            : base(O => orderIds.Contains(O.Id))
        {
            Includes.Add(O => O.Items);
            Includes.Add(O => O.DelivaryMethod!);
        }
    }
}
