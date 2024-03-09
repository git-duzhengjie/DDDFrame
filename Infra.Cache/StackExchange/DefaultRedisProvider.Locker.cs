
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Infra.Cache.StackExchange
{
    /// <summary>
    /// Default redis caching provider.
    /// </summary>
    public partial class DefaultRedisProvider : Infra.Cache.IDistributedLocker
    {
        public (Boolean Success, String LockValue) Lock(String cacheKey, Int32 timeoutSeconds = 5, Boolean autoDelay = false)
        {
            return _redisDb.Lock(cacheKey, timeoutSeconds, autoDelay);
        }

        public async Task<(Boolean Success, String LockValue)> LockAsync(String cacheKey, Int32 timeoutSeconds = 5, Boolean autoDelay = false)
        {
            return await _redisDb.LockAsync(cacheKey, timeoutSeconds, autoDelay);
        }

        public Boolean SafedUnLock(String cacheKey, String lockValue)
        {
            return _redisDb.SafedUnLock(cacheKey, lockValue);
        }

        public async Task<Boolean> SafedUnLockAsync(String cacheKey, String lockValue)
        {
            return await _redisDb.SafedUnLockAsync(cacheKey, lockValue);
        }
    }
}
