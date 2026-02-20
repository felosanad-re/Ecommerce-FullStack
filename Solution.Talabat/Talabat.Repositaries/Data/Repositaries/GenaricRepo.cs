
using Microsoft.EntityFrameworkCore;
using Talabat.Core.Entites;
using Talabat.Core.GenaricRepo;
using Talabat.Core.Specifications;
using Talabat.Repositaries.Specifications;

namespace Talabat.Repositaries.Data.Repositaries
{
    public class GenaricRepo<T> : IGenaricRepo<T> where T : ModelBase
    {
        private readonly ShopDbContext _dbContext;

        public GenaricRepo(ShopDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<T?> Get(int? id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<IReadOnlyList<T>> GetAllAsyncSpec(ISpecification<T> specification)
        {
            return await AddSpecifications(specification).ToListAsync();
        }

        public async Task<T?> GetSpec(ISpecification<T> specification)
        {
            return await AddSpecifications(specification).FirstOrDefaultAsync();
        }

        private IQueryable<T> AddSpecifications(ISpecification<T> specification)
        {
            // _dbContext.Set<Product>().include(p => p.brand).include(p => p.Category)
            return SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>(), specification);
        }



        #region Admin

        public async Task AddAsync(T entity)
            => await _dbContext.Set<T>().AddAsync(entity);

        public void Update(T entity)
            => _dbContext.Set<T>().Update(entity);

        public void delete(T entity)
        {
            entity.IsDeleted = true;
            _dbContext.Update(entity);
        }


        #endregion
    }
}
