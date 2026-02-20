using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Specifications.SpecModel
{
    public class Pagination<T>
    {
        public Pagination(int pageSize, int pageIndex, IReadOnlyList<T> products, int count)
        {
            PageSize = pageSize;
            PageIndex = pageIndex;
            Products = products;
            Count = count;
        }

        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int Count { get; set; }
        public IReadOnlyList<T> Products { get; set; }
    }
}
