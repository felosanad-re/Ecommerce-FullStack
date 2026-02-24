using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entites.Orders;

namespace Talabat.Core.Services.Contract.HubServices
{
    public interface IOrderTracingServiceHub
    {
        Task BroadcastOrderStatusChanges(int id, OrderStatus status);
    }
}
