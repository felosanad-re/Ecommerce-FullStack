using Talabat.Core.RequestModels.Import;
using Talabat.Core.ResponseModel.Import;

namespace Talabat.Core.Services.Contract.ImportServices
{
    public interface IimportService
    {
        Task<ImportToReturnDTO<DTO>> ExcelImportAsync<DTO>(ImportDTO<DTO> dTO);
    }
}
