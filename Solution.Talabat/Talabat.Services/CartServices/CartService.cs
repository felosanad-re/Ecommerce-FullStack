using Talabat.Core.Entites.Carts;
using Talabat.Core.Entites.Orders;
using Talabat.Core.Entites.Products;
using Talabat.Core.GenaricRepo;
using Talabat.Core.Services.Contract.CartServices;
using Talabat.Core.Specifications.CartParams;
using Talabat.Core.UnitOfWork;

namespace Talabat.Services.CartServices
{
    public class CartService : ICartService
    {
        private readonly IRedisRepo<Cart> _redisRepo;
        private readonly IUnitOfWork _unitOfWork;

        public CartService(IRedisRepo<Cart> redisRepo, IUnitOfWork unitOfWork)
        {
            _redisRepo = redisRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Cart> UpdateOrCreateCart(string userId, CartParam cartParam)
        {
            var key = $"Cart:{userId}"; // Set Key
            // If cart not exsist --> Create
            var cart = await _redisRepo.GetCacheAsync(key);
            if (cart is null)
            {
                cart = new Cart()
                {
                    Id = key,
                    Items = new List<CartItems>(cartParam.Items),
                };
                await _redisRepo.SetCacheAsync(cart);
            }
            else
            {
                if (cart.Items.Count > 0) {
                    foreach (var item in cartParam.Items)
                    {
                        var product = await _unitOfWork.RepositaryAsync<Product>().Get(item.Id);

                        // دور على المنتج جوه الكارت
                        var existingItem = cart.Items.FirstOrDefault(x => x.Id == item.Id);

                        if (existingItem != null)
                        {
                            // لو موجود → update count
                            existingItem.Count = item.Count;
                            existingItem.Price = product!.Price * item.Count;
                        }
                        else
                        {
                            // لو مش موجود → add
                            cart.Items.Add(new CartItems
                            {
                                Id = item.Id,
                                Name = item.Name,
                                PictureUrl = item.PictureUrl,
                                Price = item.Price,
                                Count = item.Count
                            });
                        }
                    }
                    cart.DeleveryMethodId = cartParam.DeleveryMethodId;
                }
                await _redisRepo.SetCacheAsync(cart);
                await _unitOfWork.CompleteAsync();
            }
            return cart;
        }

        public async Task<Cart?> GetCarts(string cartId)
        {
            var cart = await _redisRepo.GetCacheAsync(cartId);
            return cart;
        }

        public async Task<bool> Delete(string cartId)
        {
            var cart = await _redisRepo.GetCacheAsync(cartId);
            if (cart is null) return false;

            await _redisRepo.RemoveCacheAsync(cartId);
            return true;
        }

    }
}
