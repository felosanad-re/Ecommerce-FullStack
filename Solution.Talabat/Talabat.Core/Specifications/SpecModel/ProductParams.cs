using Talabat.Core.Entites.Products;

namespace Talabat.Core.Specifications.SpecModel
{
    public class ProductParams
    {
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
        public string? Sort { get; set; }

        private string? search;
        public string? Search
        {
            get { return search; }
            set { search = value?.ToLower(); }
        }
        #region Pagination
        private const int MaxPageSize = 12;
        private int pageSize = 12;

        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value > MaxPageSize ? MaxPageSize : value; }
        }
        public int PageIndex { get; set; } = 1;
        #endregion

        public bool? IsDeleted { get; set; }

        public StockType? StockStatus { get; set; }
    }
}
