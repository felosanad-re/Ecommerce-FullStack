using System.Linq.Expressions;
using Talabat.Core.Entites;

namespace Talabat.Core.Specifications
{
    public interface ISpecification<T> where T : ModelBase
    {
        public Expression<Func<T, bool>>? Creteria { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; set; }
        public Expression<Func<T, object>> OrderBy { get; set; }

        public Expression<Func<T, object>> OrderByDesc { get; set; }

        public int Skip { get; set; }
        public int Take { get; set; }
        public bool IsPagination { get; set; }
    }
}
