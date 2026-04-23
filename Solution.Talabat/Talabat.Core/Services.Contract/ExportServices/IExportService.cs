using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.RequestModels.Exports;

namespace Talabat.Core.Services.Contract.ExportServices
{
    public interface IExportService
    {
        Task<byte[]> ExportAsync<T>(ExportsRequest<T> request);

        Task<byte[]> ExportAsync(IEnumerable<IExportRequest> requests);

    }
}
