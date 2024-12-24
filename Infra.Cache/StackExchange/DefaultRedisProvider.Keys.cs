
using Infra.Cache.Core;
using System;
using System.Threading.Tasks;

namespace Infra.Cache.StackExchange
{
    /// <summary>
    /// Default redis caching provider.
    /// </summary>
    public partial class DefaultRedisProvider : IRedisProvider
    {
        public bool KeyDel(String cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var flag = _redisDb.KeyDelete(cacheKey);
            return flag;
        }

        public async Task<bool> KeyDelAsync(String cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var flag = await _redisDb.KeyDeleteAsync(cacheKey);
            return flag;
        }

        public bool KeyExpire(String cacheKey, int second)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var flag = _redisDb.KeyExpire(cacheKey, TimeSpan.FromSeconds(second));
            return flag;
        }

        public async Task<bool> KeyExpireAsync(String cacheKey, int second)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var flag = await _redisDb.KeyExpireAsync(cacheKey, TimeSpan.FromSeconds(second));
            return flag;
        }

        public bool KeyExists(String cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var flag = _redisDb.KeyExists(cacheKey);
            return flag;
        }

        public async Task<bool> KeyExistsAsync(String cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var flag = await _redisDb.KeyExistsAsync(cacheKey);
            return flag;
        }

        public Int64 TTL(String cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var ts = _redisDb.KeyTimeToLive(cacheKey);
            return ts.HasValue ? (Int64)ts.Value.TotalSeconds : -1;
        }

        public async Task<Int64> TTLAsync(String cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var ts = await _redisDb.KeyTimeToLiveAsync(cacheKey);
            return ts.HasValue ? (Int64)ts.Value.TotalSeconds : -1;
        }
    }
}
