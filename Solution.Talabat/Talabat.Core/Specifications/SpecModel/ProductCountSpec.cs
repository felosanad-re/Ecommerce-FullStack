using Talabat.Core.Entites.Products;

namespace Talabat.Core.Specifications.SpecModel
{
    public class ProductCountSpec : BaseSpecification<Product>
    {
        public ProductCountSpec(ProductParams productParams)
            : base(
                    P => (string.IsNullOrEmpty(productParams.Search) || P.Name.Contains(productParams.Search))
                    && (!productParams.BrandId.HasValue || P.BrandId == productParams.BrandId)
                    && (!productParams.CategoryId.HasValue || P.CategoryId == productParams.CategoryId)
                  )
        {
            
        }

    }
}
