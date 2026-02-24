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
    }
}
