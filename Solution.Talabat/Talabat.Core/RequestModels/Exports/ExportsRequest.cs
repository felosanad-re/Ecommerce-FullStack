using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.RequestModels.Exports
{
    public interface IExportRequest
    {
        string WorksheetName { get; }
        Type DataType { get; }
        Task<IReadOnlyList<object>> FetchDataAsync();
    }

    public class ExportsRequest<T> : IExportRequest
    {
        public string WorksheetName { get; set; } = "sheet1";
        public Func<Task<IReadOnlyList<T>>> DataFetcher { get; set; } = null!;
        public Type DataType => typeof(T);

        public async Task<IReadOnlyList<object>> FetchDataAsync()
        {
            var data = await DataFetcher();
            return data.Cast<object>().ToList();
        }
    }
}
