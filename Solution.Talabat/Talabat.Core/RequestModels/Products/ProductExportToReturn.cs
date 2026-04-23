using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entites.Brands;
using Talabat.Core.Entites.Categories;
using Talabat.Core.Entites.Products;
using Talabat.Core.Entites.StockTransactions;

namespace Talabat.Core.RequestModels.Products
{
    public class ProductExportToReturn
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Descripaion { get; set; }
        public string PictureUrl { get; set; }
        public decimal Price { get; set; }

        public int BrandId { get; set; }
        public string BrandName { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public int Stock { get; set; }

        public string StockType { get; set; }
    }
}
