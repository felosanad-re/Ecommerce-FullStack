using Microsoft.AspNetCore.Http;

namespace Talabat.Core.RequestModels.Import
{
    public class ImportDTO<DTO>
    {
        public IFormFile File { get; set; }

        /// <summary>
        /// Optional zip file containing product images that will be matched to order items by ProductId or ProductName.
        /// </summary>
        public IFormFile? ZipFile { get; set; }

        public ImportExcelConfig<DTO> Config { get; set; }
    }
}
