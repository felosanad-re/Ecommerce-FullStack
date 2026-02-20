using Talabat.Core.Entites.Products;

namespace Talabat.Core.Specifications.SpecModel
{
    public class ProductSpecifications : BaseSpecification<Product>
    {
        public ProductSpecifications(ProductParams productParams) 
            : base(
                    P => (string.IsNullOrEmpty(productParams.Search) || P.Name.ToLower().Contains(productParams.Search))
                    &&
                    (!productParams.BrandId.HasValue || P.BrandId == productParams.BrandId.Value)
                    &&
                    (!productParams.CategoryId.HasValue || P.CategoryId == productParams.CategoryId.Value)
                    &&
                    (!productParams.IsInStock || P.Stock > 0)
                    &&
                    (!productParams.IsDeleted.HasValue || P.IsDeleted == productParams.IsDeleted.Value)
                  )
        {
            // Includes
            AddIncludes();

            // Sort 
            if (!string.IsNullOrEmpty(productParams.Sort))
            {
                switch (productParams.Sort)
                {
                    case "PriceAsync":
                        AddOrderBy(P => P.Price);
                        break;
                    case "PriceDesc":
                        AddOrderByDesc(P => P.Price);
                        break;
                    default:
                        AddOrderBy(P => P.Name);
                        break;
                }
            }
            else AddOrderBy(P => P.Name);

            AddPagination((productParams.PageIndex - 1 ) * productParams.PageSize, productParams.PageSize);
        }

        public ProductSpecifications(int productId) 
            : base(P => P.Id == productId)
        {
            AddIncludes();
        }

        private void AddIncludes()
        {
            Includes.Add(P => P.Brand!);
            Includes.Add(P => P.Category!);
        }
    }
}
