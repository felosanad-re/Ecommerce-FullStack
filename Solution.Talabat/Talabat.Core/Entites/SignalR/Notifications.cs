using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entites.Identity;

namespace Talabat.Core.Entites.SignalR
{
    public class Notifications : ModelBase
    {
        public int OrderId { get; set; } // orderId
        public string Status { get; set; }
        public string Message { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public bool IsRead { get; set; } = false;
    }
}
