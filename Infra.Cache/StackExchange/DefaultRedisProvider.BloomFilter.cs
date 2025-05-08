
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
        public async Task<bool> BloomAddAsync(string key, string value)
        {
            return await redisDb.BloomAddAsync(key, value);
        }

        public async Task<bool[]> BloomAddAsync(string key, IEnumerable<string> values)
        {
            var redisValues = values.Select(x => (RedisValue)x);
            return await redisDb.BloomAddAsync(key, redisValues);
        }

        public async Task<bool> BloomExistsAsync(string key, string value)
        {
            return await redisDb.BloomExistsAsync(key, value);
        }

        public async Task<bool[]> BloomExistsAsync(string key, IEnumerable<string> values)
        {
            var redisValues = values.Select(x => (RedisValue)x);
            return await redisDb.BloomExistsAsync(key, redisValues);
        }

        public async Task BloomReserveAsync(string key, Double errorRate, Int32 initialCapacity)
        {
            await redisDb.BloomReserveAsync(key, errorRate, initialCapacity);
        }
    }
}
