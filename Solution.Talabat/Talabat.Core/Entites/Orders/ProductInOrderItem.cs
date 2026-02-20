using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entites.Orders
{
    public class ProductInOrderItem
    {
        public ProductInOrderItem()
        {
            
        }

        public ProductInOrderItem(int productId, string name, string pictureUrl)
        {
            ProductId = productId;
            Name = name;
            PictureUrl = pictureUrl;
        }

        public int ProductId { get; set; }
        public string Name { get; set; }
        public string PictureUrl { get; set; }
    }
}
