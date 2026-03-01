namespace Talabat.Core.Specifications.NotificationsSpec
{
    public class NotificationsParams
    {
        public bool? IsDeleted { get; set; }
        public bool? IsRead { get; set; }

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
