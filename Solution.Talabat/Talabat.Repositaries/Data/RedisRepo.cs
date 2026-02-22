using StackExchange.Redis;
using System.Text.Json;
using Talabat.Core.Entites;
using Talabat.Core.GenaricRepo;

namespace Talabat.Repositaries.Data
{
    // Can Use With Any Memory Data [Otp - Cart]
    public class RedisRepo<TRedies> : IRedisRepo<TRedies> where TRedies : BaseModelRedis
    {
        // Use Only With
        #region Services
        private readonly IDatabase _database;

        public RedisRepo(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }
        #endregion
        public async Task<TRedies?> SetCacheAsync(TRedies data) 
            // Json --> string
        {
            var Data = await _database.StringSetAsync(
                    data.Id, // Key
                    JsonSerializer.Serialize(data)
                );
            if(!Data) return null;

            return await GetCacheAsync(data.Id);
        }

        public async Task<bool> SetCacheAsync(string Key, TRedies data)
        {
            return await _database.StringSetAsync(
                Key, // initialize in cart Srvice [Cart:{userId}]
                JsonSerializer.Serialize(data), // Set data as text
                TimeSpan.FromMinutes(30)
                );
        }

        public async Task<TRedies?> GetCacheAsync(string key)
        {
            try
            {
                var data = await _database.StringGetAsync(key);
                if (data.IsNullOrEmpty) return null;
                return JsonSerializer.Deserialize<TRedies>(data!);
            }
            catch
            {
                return null; // لو Redis فشل → كمل
            }
        }

        public async Task RemoveCacheAsync(string Key)
        {
            await _database.KeyDeleteAsync(Key);
        }
    }
}
