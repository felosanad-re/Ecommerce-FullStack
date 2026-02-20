using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entites.Carts;
using Talabat.Core.Specifications.CartParams;

namespace Talabat.Core.Services.Contract.CartServices
{
    public interface ICartService
    {
        // Add OrUpdate Cart
        Task<Cart> UpdateOrCreateCart(string userId, CartParam cartParam); // save cartId for specific user by userId ==> user login to create cart

        // Get Cart
        Task<Cart?> GetCarts(string cartId);

        // Delete Cart 
        Task<bool> Delete(string cartId);
    }
}
