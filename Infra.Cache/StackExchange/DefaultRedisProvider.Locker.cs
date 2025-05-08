
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Infra.Cache.StackExchange
{
    /// <summary>
    /// Default redis caching provider.
    /// </summary>
    public partial class DefaultRedisProvider : IDistributedLocker
    {
        public (bool Success, string LockValue) Lock(string cacheKey, int timeoutSeconds = 5, bool autoDelay = false)
        {
            return redisDb.Lock(cacheKey, timeoutSeconds, autoDelay);
        }

        public async Task<(bool Success, string LockValue)> LockAsync(string cacheKey, int timeoutSeconds = 5, bool autoDelay = false)
        {
            return await redisDb.LockAsync(cacheKey, timeoutSeconds, autoDelay);
        }

        public bool SafedUnLock(string cacheKey, string lockValue)
        {
            return redisDb.SafedUnLock(cacheKey, lockValue);
        }

        public async Task<bool> SafedUnLockAsync(string cacheKey, string lockValue)
        {
            return await redisDb.SafedUnLockAsync(cacheKey, lockValue);
        }
    }
}
