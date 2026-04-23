namespace Talabat.Core.ResponseModel.Import
{
    public class OrderItemImportToReturnDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public int OrderId { get; set; }
    }
}
