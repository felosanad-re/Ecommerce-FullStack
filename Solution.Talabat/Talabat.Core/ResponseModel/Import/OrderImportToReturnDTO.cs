namespace Talabat.Core.ResponseModel.Import
{
    public class OrderImportToReturnDTO
    {
        public int Id { get; set; }
        public string BuyerEmail { get; set; }
        public string OrderStatus { get; set; }
        public string DelivaryMethodName { get; set; }
        public int? DelivaryMethodId { get; set; }
        public string AddressShiper { get; set; }
        public decimal SubTotal { get; set; }
        public string OrderDate { get; set; }
        public string? PaymentId { get; set; }
    }
}
