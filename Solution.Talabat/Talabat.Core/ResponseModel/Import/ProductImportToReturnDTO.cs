using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.ResponseModel.Import
{
    public class ProductImportToReturnDTO
    {
        public int Id { get; set; }
        [Column("Product Name")]
        public string Name { get; set; }
        [Column("Descripaion")]
        public string Descripaion { get; set; }
        [Column("Picture Url")]
        public string PictureUrl { get; set; }
        public decimal Price { get; set; }
        public int BrandId { get; set; }
        [Column("Brand Name")]
        public string Brand { get; set; }

        [Column("Category Id")]
        public int CategoryId { get; set; }
        [Column("Category Name")]
        public string Category { get; set; }

        public int Stock { get; set; }

        [Column("Stock Type")]
        public string StockType { get; set; }
    }
}
