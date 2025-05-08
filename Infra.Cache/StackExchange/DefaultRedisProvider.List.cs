
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
        public T LIndex<T>(string cacheKey, long index)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var bytes = redisDb.ListGetByIndex(cacheKey, index);
            return serializer.Deserialize<T>(bytes);
        }

        public long LLen(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            return redisDb.ListLength(cacheKey);
        }

        public T LPop<T>(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var bytes = redisDb.ListLeftPop(cacheKey);
            return serializer.Deserialize<T>(bytes);
        }

        public long LPush<T>(string cacheKey, IList<T> cacheValues)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNullAndCountGTZero(cacheValues, nameof(cacheValues));

            var list = new List<RedisValue>();

            foreach (var item in cacheValues)
            {
                list.Add(serializer.Serialize(item));
            }

            var len = redisDb.ListLeftPush(cacheKey, list.ToArray());
            return len;
        }

        public List<T> LRange<T>(string cacheKey, long start, long stop)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var list = new List<T>();

            var bytes = redisDb.ListRange(cacheKey, start, stop);

            foreach (var item in bytes)
            {
                list.Add(serializer.Deserialize<T>(item));
            }

            return list;
        }

        public long LRem<T>(string cacheKey, long count, T cacheValue)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var bytes = serializer.Serialize(cacheValue);
            return redisDb.ListRemove(cacheKey, bytes, count);
        }

        public bool LSet<T>(string cacheKey, long index, T cacheValue)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var bytes = serializer.Serialize(cacheValue);
            redisDb.ListSetByIndex(cacheKey, index, bytes);
            return true;
        }

        public bool LTrim(string cacheKey, long start, long stop)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            redisDb.ListTrim(cacheKey, start, stop);
            return true;
        }

        public long LPushX<T>(string cacheKey, T cacheValue)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var bytes = serializer.Serialize(cacheValue);
            return redisDb.ListLeftPush(cacheKey, bytes);
        }

        public long LInsertBefore<T>(string cacheKey, T pivot, T cacheValue)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var pivotbytes = serializer.Serialize(pivot);
            var cacheValuebytes = serializer.Serialize(cacheValue);
            return redisDb.ListInsertBefore(cacheKey, pivotbytes, cacheValuebytes);
        }

        public long LInsertAfter<T>(string cacheKey, T pivot, T cacheValue)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var pivotbytes = serializer.Serialize(pivot);
            var cacheValuebytes = serializer.Serialize(cacheValue);
            return redisDb.ListInsertAfter(cacheKey, pivotbytes, cacheValuebytes);
        }

        public long RPushX<T>(string cacheKey, T cacheValue)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var bytes = serializer.Serialize(cacheValue);
            return redisDb.ListRightPush(cacheKey, bytes);
        }

        public long RPush<T>(string cacheKey, IList<T> cacheValues)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNullAndCountGTZero(cacheValues, nameof(cacheValues));

            var list = new List<RedisValue>();

            foreach (var item in cacheValues)
            {
                list.Add(serializer.Serialize(item));
            }

            var len = redisDb.ListRightPush(cacheKey, list.ToArray());
            return len;
        }

        public T RPop<T>(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var bytes = redisDb.ListRightPop(cacheKey);
            return serializer.Deserialize<T>(bytes);
        }

        public async Task<T> LIndexAsync<T>(string cacheKey, long index)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var bytes = await redisDb.ListGetByIndexAsync(cacheKey, index);
            return serializer.Deserialize<T>(bytes);
        }

        public async Task<long> LLenAsync(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            return await redisDb.ListLengthAsync(cacheKey);
        }

        public async Task<T> LPopAsync<T>(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var bytes = await redisDb.ListLeftPopAsync(cacheKey);
            return serializer.Deserialize<T>(bytes);
        }

        public async Task<long> LPushAsync<T>(string cacheKey, IList<T> cacheValues)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNullAndCountGTZero(cacheValues, nameof(cacheValues));

            var list = new List<RedisValue>();

            foreach (var item in cacheValues)
            {
                list.Add(serializer.Serialize(item));
            }

            var len = await redisDb.ListLeftPushAsync(cacheKey, list.ToArray());
            return len;
        }

        public async Task<List<T>> LRangeAsync<T>(string cacheKey, long start, long stop)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var list = new List<T>();

            var bytes = await redisDb.ListRangeAsync(cacheKey, start, stop);

            foreach (var item in bytes)
            {
                list.Add(serializer.Deserialize<T>(item));
            }

            return list;
        }

        public async Task<long> LRemAsync<T>(string cacheKey, long count, T cacheValue)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var bytes = serializer.Serialize(cacheValue);
            return await redisDb.ListRemoveAsync(cacheKey, bytes, count);
        }

        public async Task<bool> LSetAsync<T>(string cacheKey, long index, T cacheValue)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var bytes = serializer.Serialize(cacheValue);
            await redisDb.ListSetByIndexAsync(cacheKey, index, bytes);
            return true;
        }

        public async Task<bool> LTrimAsync(string cacheKey, long start, long stop)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            await redisDb.ListTrimAsync(cacheKey, start, stop);
            return true;
        }

        public Task<long> LPushXAsync<T>(string cacheKey, T cacheValue)
        {
            //ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            //var bytes = _serializer.Serialize(cacheValue);
            //return await _cache.ListLeftPushAsync(cacheKey, bytes);
            throw new NotImplementedException();
        }

        public async Task<long> LInsertBeforeAsync<T>(string cacheKey, T pivot, T cacheValue)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var pivotbytes = serializer.Serialize(pivot);
            var cacheValuebytes = serializer.Serialize(cacheValue);
            return await redisDb.ListInsertBeforeAsync(cacheKey, pivotbytes, cacheValuebytes);
        }

        public async Task<long> LInsertAfterAsync<T>(string cacheKey, T pivot, T cacheValue)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var pivotbytes = serializer.Serialize(pivot);
            var cacheValuebytes = serializer.Serialize(cacheValue);
            return await redisDb.ListInsertAfterAsync(cacheKey, pivotbytes, cacheValuebytes);
        }

        public Task<long> RPushXAsync<T>(string cacheKey, T cacheValue)
        {
            //ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            //var bytes = _serializer.Serialize(cacheValue);
            //return await _cache.ListRightPushAsync(cacheKey, bytes);
            throw new NotImplementedException();
        }

        public async Task<long> RPushAsync<T>(string cacheKey, IList<T> cacheValues)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNullAndCountGTZero(cacheValues, nameof(cacheValues));

            var list = new List<RedisValue>();

            foreach (var item in cacheValues)
            {
                list.Add(serializer.Serialize(item));
            }

            var len = await redisDb.ListRightPushAsync(cacheKey, list.ToArray());
            return len;
        }

        public async Task<T> RPopAsync<T>(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var bytes = await redisDb.ListRightPopAsync(cacheKey);
            return serializer.Deserialize<T>(bytes);
        }
    }
}
