using Talabat.Core.Entites.Products;
using Talabat.Core.RequestModels;
using Talabat.Core.Specifications.SpecModel;

namespace Talabat.Core.Services.Contract.ProductServices
{
    public interface IProductService
    {
        // Get All Products
        Task<IReadOnlyList<Product>> GetProductsAsync(ProductParams productParams);
        // Get Product By Id
        Task<Product?> GetProductAsync(int productId);

        // Get Count 
        Task<int> GetProductCountAsync(ProductParams productParams);

        #region Special For Admin

        /*   Admin    */

        // Add Product
        Task<Product?> AddProductAsync(AddProductRequest addProductRequest);
        
        // Update Product
        Task<Product?> UpdateProduct(UpdateProductRequest updateProductRequest);
        
        // Delete Product 
        Task<bool> DeleteProduct(int productId);
        #endregion
    }
}
