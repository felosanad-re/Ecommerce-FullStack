namespace Talabat.Core.ResponseModel.Import
{
    public class OrderImportResultDTO
    {
        public int TotalRows { get; set; }
        public int AddedCount { get; set; }
        public int UpdatedCount { get; set; }
        public int SkippedDuplicates { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<OrderImportToReturnDTO> Orders { get; set; } = new();
        public List<OrderItemImportToReturnDTO> Items { get; set; } = new();
        public string Message => $"Total Rows is {TotalRows}, Added {AddedCount}, Updated {UpdatedCount} And skipped {SkippedDuplicates}";
    }
}
