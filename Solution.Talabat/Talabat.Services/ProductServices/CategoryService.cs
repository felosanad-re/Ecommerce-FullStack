using AutoMapper;
using Talabat.Core.Entites.Categories;
using Talabat.Core.RequestModels.CategoriesRequests;
using Talabat.Core.Services.Contract.ProductServices;
using Talabat.Core.UnitOfWork;

namespace Talabat.Services.ProductServices
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<Category>> GetAll()
    => await _unitOfWork.RepositaryAsync<Category>().GetAllAsync();

        public async Task<Category?> GetAsync(int id)
            => await _unitOfWork.RepositaryAsync<Category>().Get(id);

        public async Task<Category?> AddCategoryAsync(AddCategoryRequest addCategoryRequest)
        {
            var newCategory = _mapper.Map<Category>(addCategoryRequest);
            await _unitOfWork.RepositaryAsync<Category>().AddAsync(newCategory);
            await _unitOfWork.CompleteAsync();
            return await GetAsync(newCategory.Id);
        }

        public async Task<Category?> UpdateCategoryAsync(UpdateCategoryRequest updateCategoryRequest)
        {
            var category = await _unitOfWork.RepositaryAsync<Category>().Get(updateCategoryRequest.Id);
            if (category is null) return null;

            category.Name = updateCategoryRequest.Name;
            await _unitOfWork.CompleteAsync();
            return await GetAsync(category.Id);
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _unitOfWork.RepositaryAsync<Category>().Get(id);
            _unitOfWork.RepositaryAsync<Category>().delete(category);
            await _unitOfWork.CompleteAsync();
        }
    }
}
