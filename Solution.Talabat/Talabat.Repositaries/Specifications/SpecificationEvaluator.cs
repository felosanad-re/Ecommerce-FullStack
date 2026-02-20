using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entites;
using Talabat.Core.Specifications;

namespace Talabat.Repositaries.Specifications
{
    public static class SpecificationEvaluator<T> where T : ModelBase
    {
        public static IQueryable<T> GetQuery(IQueryable<T> input, ISpecification<T> specification)
        {
            // querry = DbContext.Set<T>()
            var query = input; // _dbContext.Set<Product>()

            // Set Creteria
            if(specification.Creteria is not null)
                query = query.Where(specification.Creteria); //_dbContext.Set<Product>()

            // Set Order By
            if(specification.OrderBy is not null)
                query = query.OrderBy(specification.OrderBy);
            if(specification.OrderByDesc is not null)
                query = query.OrderByDescending(specification.OrderByDesc);

            // Add Pagination 
            if (specification.IsPagination)
                query = query.Skip(specification.Skip).Take(specification.Take);
            // Set Includes
            query = specification.Includes.Aggregate(query, (BaseQuery, nextQuery) => BaseQuery.Include(nextQuery));
            // query = _dbContext.Set<Product>().where(P => P.BrandId == 3 && P => P.CategoryId == 3).include(p => p.brand).include(p => p.Category)
            return query;
        }
    }
}
