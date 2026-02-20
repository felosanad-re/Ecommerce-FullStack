using Talabat.Core.Entites;
using Talabat.Core.Specifications;

namespace Talabat.Core.GenaricRepo
{
    public interface IGenaricRepo<T> where T : ModelBase
    {
        // Get All
        Task<IReadOnlyList<T>> GetAllAsync();

        // Get 
        Task<T?> Get(int? id);

        // Get All Spec
        Task<IReadOnlyList<T>> GetAllAsyncSpec(ISpecification<T> specification);

        // Get Spec
        Task<T?> GetSpec(ISpecification<T> specification);

        // Add 
        Task AddAsync(T entity);

        //Update
        void Update(T entity);

        // delete
        void delete(T entity);
    }
}
