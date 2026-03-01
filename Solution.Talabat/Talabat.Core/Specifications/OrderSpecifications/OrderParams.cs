using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Specifications.OrderSpecifications
{
    public class OrderParams
    {
		private const int MaxPageSize = 10;
		private int pageSize = 10;

		public int PageSize
		{
			get { return pageSize; }
			set { pageSize = value > MaxPageSize ? MaxPageSize : value; }
		}

		public int PageIndex { get; set; } = 1;

    }
}
