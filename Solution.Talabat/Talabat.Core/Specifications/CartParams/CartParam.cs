using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entites.Carts;

namespace Talabat.Core.Specifications.CartParams
{
    public class CartParam
    {
        //public string Id { get; set; }

        public List<CartItems> Items { get; set; }

        public int? DeleveryMethodId { get; set; }
    }
}
