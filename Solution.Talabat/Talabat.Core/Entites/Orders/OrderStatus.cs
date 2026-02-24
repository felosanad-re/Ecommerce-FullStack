using System.Runtime.Serialization;

namespace Talabat.Core.Entites.Orders
{
    public enum OrderStatus
    {
        [EnumMember(Value = "Pending")]
        Pending = 1,
        [EnumMember(Value = "Payment Succedded")]
        PaymentSuccedded,
        [EnumMember(Value = "Payment Faild")]
        PaymentFaild,
        [EnumMember(Value = "Preparing Order")]
        Preparing,
        [EnumMember(Value = "Order Out For Delivary")]
        OutForDelivery,
        [EnumMember(Value = "Order Arrived")]
        Delivaery,
        [EnumMember(Value = "Order Cancellded")]
        Cancelled
    }
}
