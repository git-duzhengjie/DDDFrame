
using Infra.Cache.Core;
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
    public partial class DefaultRedisProvider : IRedisProvider
    {
        public Int64 SAdd<T>(string cacheKey, IList<T> cacheValues, TimeSpan? expiration = null)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNullAndCountGTZero(cacheValues, nameof(cacheValues));

            var list = new List<RedisValue>();

            foreach (var item in cacheValues)
            {
                list.Add(serializer.Serialize(item));
            }

            var len = redisDb.SetAdd(cacheKey, list.ToArray());

            if (expiration.HasValue)
            {
                redisDb.KeyExpire(cacheKey, expiration.Value);
            }

            return len;
        }

        public Int64 SCard(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var len = redisDb.SetLength(cacheKey);
            return len;
        }

        public bool SIsMember<T>(string cacheKey, T cacheValue)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var bytes = serializer.Serialize(cacheValue);

            var flag = redisDb.SetContains(cacheKey, bytes);
            return flag;
        }

        public List<T> SMembers<T>(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var list = new List<T>();

            var bytes = redisDb.SetMembers(cacheKey);

            foreach (var item in bytes)
            {
                list.Add(serializer.Deserialize<T>(item));
            }

            return list;
        }

        public T SPop<T>(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var bytes = redisDb.SetPop(cacheKey);

            return serializer.Deserialize<T>(bytes);
        }

        public List<T> SRandMember<T>(string cacheKey, int count = 1)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var list = new List<T>();

            var bytes = redisDb.SetRandomMembers(cacheKey, count);

            foreach (var item in bytes)
            {
                list.Add(serializer.Deserialize<T>(item));
            }

            return list;
        }

        public Int64 SRem<T>(string cacheKey, IList<T> cacheValues = null)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var len = 0L;

            if (cacheValues != null && cacheValues.Any())
            {
                var bytes = new List<RedisValue>();

                foreach (var item in cacheValues)
                {
                    bytes.Add(serializer.Serialize<T>(item));
                }

                len = redisDb.SetRemove(cacheKey, bytes.ToArray());
            }
            else
            {
                var flag = redisDb.KeyDelete(cacheKey);
                len = flag ? 1 : 0;
            }

            return len;
        }

        public async Task<Int64> SAddAsync<T>(string cacheKey, IList<T> cacheValues, TimeSpan? expiration = null)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNullAndCountGTZero(cacheValues, nameof(cacheValues));

            var list = new List<RedisValue>();

            foreach (var item in cacheValues)
            {
                list.Add(serializer.Serialize(item));
            }

            var len = await redisDb.SetAddAsync(cacheKey, list.ToArray());

            if (expiration.HasValue)
            {
                await redisDb.KeyExpireAsync(cacheKey, expiration.Value);
            }

            return len;
        }

        public async Task<long> SCardAsync(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var len = await redisDb.SetLengthAsync(cacheKey);
            return len;
        }

        public async Task<bool> SIsMemberAsync<T>(string cacheKey, T cacheValue)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var bytes = serializer.Serialize(cacheValue);

            var flag = await redisDb.SetContainsAsync(cacheKey, bytes);
            return flag;
        }

        public async Task<List<T>> SMembersAsync<T>(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var list = new List<T>();

            var vals = await redisDb.SetMembersAsync(cacheKey);

            foreach (var item in vals)
            {
                list.Add(serializer.Deserialize<T>(item));
            }

            return list;
        }

        public async Task<T> SPopAsync<T>(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var bytes = await redisDb.SetPopAsync(cacheKey);

            return serializer.Deserialize<T>(bytes);
        }

        public async Task<List<T>> SRandMemberAsync<T>(string cacheKey, int count = 1)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var list = new List<T>();

            var bytes = await redisDb.SetRandomMembersAsync(cacheKey, count);

            foreach (var item in bytes)
            {
                list.Add(serializer.Deserialize<T>(item));
            }

            return list;
        }

        public async Task<long> SRemAsync<T>(string cacheKey, IList<T> cacheValues = null)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var len = 0L;

            if (cacheValues != null && cacheValues.Any())
            {
                var bytes = new List<RedisValue>();

                foreach (var item in cacheValues)
                {
                    bytes.Add(serializer.Serialize<T>(item));
                }

                len = await redisDb.SetRemoveAsync(cacheKey, bytes.ToArray());
            }
            else
            {
                var flag = await redisDb.KeyDeleteAsync(cacheKey);
                len = flag ? 1 : 0;
            }

            return len;
        }
    }
}
