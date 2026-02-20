using Talabat.Core.Entites.Categories;
using Talabat.Core.RequestModels.CategoriesRequests;

namespace Talabat.Core.Services.Contract.ProductServices
{
    public interface ICategoryService
    {
        Task<IReadOnlyList<Category>> GetAll();
        Task<Category?> GetAsync(int id);

        Task<Category?> AddCategoryAsync(AddCategoryRequest addCategoryRequest);

        Task<Category?> UpdateCategoryAsync(UpdateCategoryRequest updateCategoryRequest);

        Task DeleteCategoryAsync(int id);
    }
}
