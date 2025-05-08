
using Infra.Cache.Core;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infra.Cache.StackExchange
{
    /// <summary>
    /// Default redis caching provider.
    /// </summary>
    public partial class DefaultRedisProvider : IRedisProvider
    {
        public bool PfAdd<T>(string cacheKey, List<T> values)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNullAndCountGTZero(values, nameof(values));

            var list = new List<RedisValue>();

            foreach (var item in values)
            {
                list.Add(serializer.Serialize(item));
            }

            var res = redisDb.HyperLogLogAdd(cacheKey, list.ToArray());
            return res;
        }

        public async Task<bool> PfAddAsync<T>(string cacheKey, List<T> values)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNullAndCountGTZero(values, nameof(values));

            var list = new List<RedisValue>();

            foreach (var item in values)
            {
                list.Add(serializer.Serialize(item));
            }

            var res = await redisDb.HyperLogLogAddAsync(cacheKey, list.ToArray());
            return res;
        }

        public long PfCount(List<string> cacheKeys)
        {
            ArgumentCheck.NotNullAndCountGTZero(cacheKeys, nameof(cacheKeys));

            var list = new List<RedisKey>();

            foreach (var item in cacheKeys)
            {
                list.Add(item);
            }

            var res = redisDb.HyperLogLogLength(list.ToArray());
            return res;
        }

        public async Task<Int64> PfCountAsync(List<string> cacheKeys)
        {
            ArgumentCheck.NotNullAndCountGTZero(cacheKeys, nameof(cacheKeys));

            var list = new List<RedisKey>();

            foreach (var item in cacheKeys)
            {
                list.Add(item);
            }

            var res = await redisDb.HyperLogLogLengthAsync(list.ToArray());
            return res;
        }

        public bool PfMerge(string destKey, List<string> sourceKeys)
        {
            ArgumentCheck.NotNullOrWhiteSpace(destKey, nameof(destKey));
            ArgumentCheck.NotNullAndCountGTZero(sourceKeys, nameof(sourceKeys));

            var list = new List<RedisKey>();

            foreach (var item in sourceKeys)
            {
                list.Add(item);
            }

            redisDb.HyperLogLogMerge(destKey, list.ToArray());
            return true;
        }

        public async Task<bool> PfMergeAsync(string destKey, List<string> sourceKeys)
        {
            ArgumentCheck.NotNullOrWhiteSpace(destKey, nameof(destKey));
            ArgumentCheck.NotNullAndCountGTZero(sourceKeys, nameof(sourceKeys));

            var list = new List<RedisKey>();

            foreach (var item in sourceKeys)
            {
                list.Add(item);
            }

            await redisDb.HyperLogLogMergeAsync(destKey, list.ToArray());
            return true;
        }
    }
}
