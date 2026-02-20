using Talabat.Core.Entites;

namespace Talabat.Core.GenaricRepo
{
    public interface IRedisRepo<TRedies> where TRedies : BaseModelRedis
    {
        // Set Key Value --> Json | string
        Task<TRedies?> SetCacheAsync(TRedies data); // Set OTP
        Task<bool> SetCacheAsync(string Key, TRedies data); // For Cart

        // Get
        Task<TRedies?> GetCacheAsync(string key);

        // Delete
        Task RemoveCacheAsync(string Key);
    }
}
