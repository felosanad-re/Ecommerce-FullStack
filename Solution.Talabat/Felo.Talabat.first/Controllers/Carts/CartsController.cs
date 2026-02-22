using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
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
        private readonly ILogger<CartsController> _logger;
        private readonly IDatabase _database;

        public CartsController(ICartService cartServices, ILogger<CartsController> logger, IConnectionMultiplexer redis)
        {
            _cartServices = cartServices;
            _logger = logger;
            _database = redis.GetDatabase();
        }
        #endregion

        #region Create Or Update Cart
        [HttpPost("UpdateOrCreateCart")] //Post: /api/Carts/UpdateOrCreateCart
        [Authorize]
        public async Task<ActionResult<Cart>> UpdateOrCreateCart([FromBody] CartParam cartParam)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var cart = await _cartServices.UpdateOrCreateCart(userId!, cartParam);
                _logger.LogInformation("UpdateOrCreateCart called with user: {UserId}", userId);

            return Ok(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update cart");
                return StatusCode(500, new { error = ex.Message });  // مؤقتاً
            }
        }
        #endregion

        #region Get Cart Details
        [HttpGet("CartDetails")] // Get: /api/Carts/CartDetails
        public async Task<ActionResult<Cart>> GetCartDetails()
        {
            try
            {
                string cartId = GetCartId();
                if (string.IsNullOrEmpty(cartId))
                    return BadRequest("CartId is empty");

                var cart = await _cartServices.GetCarts(cartId);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
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


        [HttpGet("test-redis")]
        public async Task<IActionResult> TestRedis()
        {
            try
            {
                await _database.PingAsync();  // أو StringSetAsync + StringGetAsync بسيط
                return Ok("Redis connected successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Redis error: {ex.Message} | {ex.GetType().Name}");
            }
        }
    }
}
