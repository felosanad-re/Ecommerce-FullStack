namespace Talabat.Core.RequestModels.Import
{
    public class ImportExcelConfig<DTO>
    {
        public string SheetName { get; set; } = "sheet 1";
        public int StartRow { get; set; } = 2;
        public bool HasHeader { get; set; } = true;

        public Dictionary<string, string> ColumnMapping { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    }
}
