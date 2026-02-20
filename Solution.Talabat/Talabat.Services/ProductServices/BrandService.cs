using AutoMapper;
using Talabat.Core.Entites.Brands;
using Talabat.Core.RequestModels.BrandRequests;
using Talabat.Core.Services.Contract.ProductServices;
using Talabat.Core.UnitOfWork;

namespace Talabat.Services.ProductServices
{
    public class BrandService : IBrandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BrandService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<Brand>> GetAllAsync()
            => await _unitOfWork.RepositaryAsync<Brand>().GetAllAsync();

        public async Task<Brand?> GetAsync(int id)
          =>  await _unitOfWork.RepositaryAsync<Brand>().Get(id);

        public async Task<Brand?> AddAsync(BrandRequest brand)
        {
            var addedbrand = _mapper.Map<Brand>(brand);
            if (addedbrand == null) return null;
            await _unitOfWork.RepositaryAsync<Brand>().AddAsync(addedbrand);
            await _unitOfWork.CompleteAsync();
            return await GetAsync(addedbrand.Id);
        }

        public async Task<Brand?> UpdateAsync(UpdateBrandRequest updateBrandRequest)
        {
            var updatedBrand = await _unitOfWork.RepositaryAsync<Brand>().Get(updateBrandRequest.Id);
            if (updatedBrand is null) return null;
            updatedBrand.Name = updateBrandRequest.Name;
            await _unitOfWork.CompleteAsync();
            return await GetAsync(updatedBrand.Id);
        }

        public async Task DeleteBrandAsync(int id)
        {
            var brand = await _unitOfWork.RepositaryAsync<Brand>().Get(id);
            _unitOfWork.RepositaryAsync<Brand>().delete(brand);
            await _unitOfWork.CompleteAsync();
        }
    }
}
