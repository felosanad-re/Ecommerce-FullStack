using Talabat.Core.Entites;
using Talabat.Core.GenaricRepo;

namespace Talabat.Core.UnitOfWork
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IGenaricRepo<TEntity> RepositaryAsync<TEntity>() where TEntity : ModelBase;
        Task<int> CompleteAsync();
    }
}
