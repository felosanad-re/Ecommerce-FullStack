using System.Linq.Expressions;
using Talabat.Core.Entites;

namespace Talabat.Core.Specifications
{
    public class BaseSpecification<T> : ISpecification<T> where T : ModelBase
    {
        public Expression<Func<T, bool>>? Creteria { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();

        public Expression<Func<T, object>> OrderBy { get; set; }

        public Expression<Func<T, object>> OrderByDesc { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public bool IsPagination { get; set; }

        // Set Creatria null
        public BaseSpecification()
        {
            
        }

        // Set Creatria
        public BaseSpecification(Expression<Func<T, bool>> expression)
        {
            Creteria = expression; // P => P.Name == Search
        }

        public void AddOrderBy(Expression<Func<T, object>> expression)
        {
            OrderBy = expression;
        }

        public void AddOrderByDesc(Expression<Func<T, object>> expression)
        {
            OrderByDesc = expression; // P => P.Price
        }

        public void AddPagination(int skip, int take)
        {
            IsPagination = true;
            Skip = skip;
            Take = take;
        }
    }
}
