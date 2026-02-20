using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entites;
using Talabat.Core.GenaricRepo;
using Talabat.Core.UnitOfWork;
using Talabat.Repositaries.Data.Repositaries;

namespace Talabat.Repositaries.Data.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ShopDbContext _dbContext;
        private Hashtable _repositary;

        public UnitOfWork(ShopDbContext dbContext)
        {
            _dbContext = dbContext;
            _repositary = new Hashtable();
        }

        public IGenaricRepo<TEntity> RepositaryAsync<TEntity>() where TEntity : ModelBase
        {
            var key = typeof(TEntity);
            if (!_repositary.ContainsKey(key)) {
                var repositary = new GenaricRepo<TEntity>(_dbContext);
                _repositary.Add(key, repositary);
            }
            return _repositary[key] as IGenaricRepo<TEntity>;
        }

        public async Task<int> CompleteAsync()
            => await _dbContext.SaveChangesAsync();

        public async ValueTask DisposeAsync()
            => await _dbContext.DisposeAsync();

    }
}
