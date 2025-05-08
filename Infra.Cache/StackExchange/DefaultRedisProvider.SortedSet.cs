
using Infra.Cache.Core;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infra.Cache.StackExchange
{
    /// <summary>
    /// Default redis caching provider.
    /// </summary>
    public partial class DefaultRedisProvider : IRedisProvider
    {
        public long ZAdd<T>(string cacheKey, Dictionary<T, double> cacheValues) where T:notnull
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var param = new List<SortedSetEntry>();

            foreach (var item in cacheValues)
            {
                param.Add(new SortedSetEntry(serializer.Serialize(item.Key), item.Value));
            }

            var len = redisDb.SortedSetAdd(cacheKey, param.ToArray());

            return len;
        }

        public long ZCard(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var len = redisDb.SortedSetLength(cacheKey);
            return len;
        }

        public long ZCount(string cacheKey, double min, double max)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var len = redisDb.SortedSetLengthByValue(cacheKey, min, max);
            return len;
        }

        public double ZIncrBy(string cacheKey, string field, double val = 1)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNullOrWhiteSpace(field, nameof(field));

            var value = redisDb.SortedSetIncrement(cacheKey, field, val);
            return value;
        }

        public long ZLexCount(string cacheKey, string min, string max)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var len = redisDb.SortedSetLengthByValue(cacheKey, min, max);
            return len;
        }

        public List<T> ZRange<T>(string cacheKey, long start, long stop)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var list = new List<T>();

            var bytes = redisDb.SortedSetRangeByRank(cacheKey, start, stop);

            foreach (var item in bytes)
            {
                list.Add(serializer.Deserialize<T>(item));
            }

            return list;
        }

        public long? ZRank<T>(string cacheKey, T cacheValue)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var bytes = serializer.Serialize(cacheValue);

            var index = redisDb.SortedSetRank(cacheKey, bytes);

            return index;
        }

        public long ZRem<T>(string cacheKey, IList<T> cacheValues)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var bytes = new List<RedisValue>();

            foreach (var item in cacheValues)
            {
                bytes.Add(serializer.Serialize(item));
            }

            var len = redisDb.SortedSetRemove(cacheKey, bytes.ToArray());

            return len;
        }

        public double? ZScore<T>(string cacheKey, T cacheValue)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var bytes = serializer.Serialize(cacheValue);

            var score = redisDb.SortedSetScore(cacheKey, bytes);

            return score;
        }

        public async Task<long> ZAddAsync<T>(string cacheKey, Dictionary<T, double> cacheValues)where T:notnull
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var param = new List<SortedSetEntry>();

            foreach (var item in cacheValues)
            {
                param.Add(new SortedSetEntry(serializer.Serialize(item.Key), item.Value));
            }

            var len = await redisDb.SortedSetAddAsync(cacheKey, [.. param]);

            return len;
        }

        public async Task<long> ZCardAsync(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var len = await redisDb.SortedSetLengthAsync(cacheKey);
            return len;
        }

        public async Task<long> ZCountAsync(string cacheKey, double min, double max)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var len = await redisDb.SortedSetLengthByValueAsync(cacheKey, min, max);
            return len;
        }

        public async Task<double> ZIncrByAsync(string cacheKey, string field, double val = 1)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNullOrWhiteSpace(field, nameof(field));

            var value = await redisDb.SortedSetIncrementAsync(cacheKey, field, val);
            return value;
        }

        public async Task<long> ZLexCountAsync(string cacheKey, string min, string max)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var len = await redisDb.SortedSetLengthByValueAsync(cacheKey, min, max);
            return len;
        }

        public async Task<List<T>> ZRangeAsync<T>(string cacheKey, long start, long stop)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var list = new List<T>();

            var bytes = await redisDb.SortedSetRangeByRankAsync(cacheKey, start, stop);

            foreach (var item in bytes)
            {
                list.Add(serializer.Deserialize<T>(item));
            }

            return list;
        }

        public async Task<long?> ZRankAsync<T>(string cacheKey, T cacheValue)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var bytes = serializer.Serialize(cacheValue);

            var index = await redisDb.SortedSetRankAsync(cacheKey, bytes);

            return index;
        }

        public async Task<long> ZRemAsync<T>(string cacheKey, IList<T> cacheValues)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var bytes = new List<RedisValue>();

            foreach (var item in cacheValues)
            {
                bytes.Add(serializer.Serialize(item));
            }

            var len = await redisDb.SortedSetRemoveAsync(cacheKey, bytes.ToArray());

            return len;
        }

        public async Task<double?> ZScoreAsync<T>(string cacheKey, T cacheValue)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var bytes = serializer.Serialize(cacheValue);

            var score = await redisDb.SortedSetScoreAsync(cacheKey, bytes);

            return score;
        }
    }
}
