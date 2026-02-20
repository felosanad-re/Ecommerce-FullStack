using Talabat.Core.Entites.Brands;
using Talabat.Core.RequestModels.BrandRequests;

namespace Talabat.Core.Services.Contract.ProductServices
{
    public interface IBrandService
    {
        Task<IReadOnlyList<Brand>> GetAllAsync();

        Task<Brand?> GetAsync(int id);

        // Add
        Task<Brand?> AddAsync(BrandRequest brand);

        // Update
        Task<Brand?> UpdateAsync(UpdateBrandRequest updateBrandRequest);
        //Delete

        Task DeleteBrandAsync(int id);
    }
}
