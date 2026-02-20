using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.Core.Entites.Carts;
using Talabat.Core.Services.Contract.CartServices;
using Talabat.Core.Specifications.CartParams;

namespace Felo.Talabat.Api.Controllers.Carts
{
    public class CartsController : BaseController
    {
        #region Services

        private readonly ICartService _cartServices;

        public CartsController(ICartService cartServices)
        {
            _cartServices = cartServices;
        }
        #endregion

        #region Create Or Update Cart
        [HttpPost("UpdateOrCreateCart")] //Post: /api/Carts/UpdateOrCreateCart
        [Authorize]
        public async Task<ActionResult<Cart>> UpdateOrCreateCart([FromBody] CartParam cartParam)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _cartServices.UpdateOrCreateCart(userId!, cartParam);

            return Ok(cart);
        }
        #endregion

        #region Get Cart Details
        [HttpGet("CartDetails")] // Get: /api/Carts/CartDetails
        public async Task<ActionResult<Cart>> GetCartDetails()
        {
            string cartId = GetCartId();
            var cart = await _cartServices.GetCarts(cartId);
            return Ok(cart);
        }
        #endregion

        #region Delete Cart

        [HttpDelete("DeleteCart")] // Delete: /api/Carts/DeleteCart
        public async Task<ActionResult> DeleteCart()
        {
            string cartId = GetCartId();
            await _cartServices.Delete(cartId);
            return Ok(new
            {
                message = "Cart Delete Succsedded"
            });
        }
        #endregion

        #region Fun --> Get CartId
        // Get CartId For Details And Delete
        private string GetCartId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartId = $"Cart:{userId}";
            return cartId;
        } 
        #endregion
    }
}
