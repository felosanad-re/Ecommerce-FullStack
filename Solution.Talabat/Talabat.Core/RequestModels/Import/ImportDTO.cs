using Microsoft.AspNetCore.Http;

namespace Talabat.Core.RequestModels.Import
{
    public class ImportDTO<DTO>
    {
        public IFormFile File { get; set; }

        public ImportExcelConfig<DTO> Config { get; set; }
    }
}
