
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infra.Cache.StackExchange
{
    /// <summary>
    /// Default redis caching provider.
    /// </summary>
    public partial class DefaultRedisProvider : Infra.Cache.IRedisProvider
    {
        public async Task<Boolean> BloomAddAsync(String key, String value)
        {
            return await _redisDb.BloomAddAsync(key, value);
        }

        public async Task<Boolean[]> BloomAddAsync(String key, IEnumerable<String> values)
        {
            var redisValues = values.Select(x => (RedisValue)x);
            return await _redisDb.BloomAddAsync(key, redisValues);
        }

        public async Task<Boolean> BloomExistsAsync(String key, String value)
        {
            return await _redisDb.BloomExistsAsync(key, value);
        }

        public async Task<Boolean[]> BloomExistsAsync(String key, IEnumerable<String> values)
        {
            var redisValues = values.Select(x => (RedisValue)x);
            return await _redisDb.BloomExistsAsync(key, redisValues);
        }

        public async Task BloomReserveAsync(String key, Double errorRate, Int32 initialCapacity)
        {
            await _redisDb.BloomReserveAsync(key, errorRate, initialCapacity);
        }
    }
}
