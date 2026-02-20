using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entites.Orders
{
    public enum OrderStatus
    {
        [EnumMember(Value = "Pending")]
        Pending = 1,
        [EnumMember(Value = "Payment Succedded")]
        PaymentSuccedded,
        [EnumMember(Value = "Payment Faild")]
        PaymentFaild
    }
}
