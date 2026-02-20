using AutoMapper;
using Talabat.Core.Entites.Products;
using Talabat.Core.RequestModels;
using Talabat.Core.Services.Contract.ProductServices;
using Talabat.Core.Specifications.SpecModel;
using Talabat.Core.UnitOfWork;

namespace Talabat.Services.ProductServices
{
    public class ProductService : IProductService
    {
        #region Services

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        #endregion

        #region User Services
        // Get Products
        public async Task<IReadOnlyList<Product>> GetProductsAsync(ProductParams productParams)
        {
            var spec = new ProductSpecifications(productParams);
            var products = await _unitOfWork.RepositaryAsync<Product>().GetAllAsyncSpec(spec);
            return products;
        }

        // Get Product By Id
        public async Task<Product?> GetProductAsync(int productId)
        {
            var spec = new ProductSpecifications(productId);
            var product = await _unitOfWork.RepositaryAsync<Product>().GetSpec(spec);
            return product;
        }

        // Get Product Count Before Pagination
        public async Task<int> GetProductCountAsync(ProductParams productParams)
        {
            var spec = new ProductCountSpec(productParams);
            var count = await _unitOfWork.RepositaryAsync<Product>().GetAllAsyncSpec(spec);

            return count.Count;
        } 
        #endregion

        #region Admin Region

        public async Task<Product?> AddProductAsync(AddProductRequest addProductRequest)
        {
            // Auto Mapper
            var newProduct = _mapper.Map<Product>(addProductRequest);
            await _unitOfWork.RepositaryAsync<Product>().AddAsync(newProduct);
            await _unitOfWork.CompleteAsync();

            return await GetProductAsync(newProduct.Id);
        }

        public async Task<Product?> UpdateProduct(UpdateProductRequest updateProductRequest)
        {
            var updateProduct = await _unitOfWork.RepositaryAsync<Product>().Get(updateProductRequest.Id);
            if (updateProduct == null) return null;
             // Auto Mapper
            _mapper.Map(updateProductRequest, updateProduct);
            await _unitOfWork.CompleteAsync();
            return await GetProductAsync(updateProduct.Id);
        }

        public async Task<bool> DeleteProduct(int productId)
        {
            var product = await _unitOfWork.RepositaryAsync<Product>().Get(productId);
            if(product == null) return false;
            _unitOfWork.RepositaryAsync<Product>().delete(product);
            await _unitOfWork.CompleteAsync();
            return true;
        }
        #endregion
    }
}
