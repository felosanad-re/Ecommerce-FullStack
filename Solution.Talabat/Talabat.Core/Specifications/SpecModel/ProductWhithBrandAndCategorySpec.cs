using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entites.Products;

namespace Talabat.Core.Specifications.SpecModel
{
    public class ProductWhithBrandAndCategorySpec : BaseSpecification<Product>
    {
        public ProductWhithBrandAndCategorySpec(int productId)
            :base(P => P.Id == productId)
        {
            Includes.Add(P => P.Brand);
            Includes.Add(P => P.Category);
        }
    }
}
