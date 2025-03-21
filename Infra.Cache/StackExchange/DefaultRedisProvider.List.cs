﻿
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
        public T LIndex<T>(String cacheKey, long index)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var bytes = _redisDb.ListGetByIndex(cacheKey, index);
            return _serializer.Deserialize<T>(bytes);
        }

        public long LLen(String cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            return _redisDb.ListLength(cacheKey);
        }

        public T LPop<T>(String cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var bytes = _redisDb.ListLeftPop(cacheKey);
            return _serializer.Deserialize<T>(bytes);
        }

        public long LPush<T>(String cacheKey, IList<T> cacheValues)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNullAndCountGTZero(cacheValues, nameof(cacheValues));

            var list = new List<RedisValue>();

            foreach (var item in cacheValues)
            {
                list.Add(_serializer.Serialize(item));
            }

            var len = _redisDb.ListLeftPush(cacheKey, list.ToArray());
            return len;
        }

        public List<T> LRange<T>(String cacheKey, long start, long stop)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var list = new List<T>();

            var bytes = _redisDb.ListRange(cacheKey, start, stop);

            foreach (var item in bytes)
            {
                list.Add(_serializer.Deserialize<T>(item));
            }

            return list;
        }

        public long LRem<T>(String cacheKey, long count, T cacheValue)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var bytes = _serializer.Serialize(cacheValue);
            return _redisDb.ListRemove(cacheKey, bytes, count);
        }

        public bool LSet<T>(String cacheKey, long index, T cacheValue)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var bytes = _serializer.Serialize(cacheValue);
            _redisDb.ListSetByIndex(cacheKey, index, bytes);
            return true;
        }

        public bool LTrim(String cacheKey, long start, long stop)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            _redisDb.ListTrim(cacheKey, start, stop);
            return true;
        }

        public long LPushX<T>(String cacheKey, T cacheValue)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var bytes = _serializer.Serialize(cacheValue);
            return _redisDb.ListLeftPush(cacheKey, bytes);
        }

        public long LInsertBefore<T>(String cacheKey, T pivot, T cacheValue)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var pivotbytes = _serializer.Serialize(pivot);
            var cacheValuebytes = _serializer.Serialize(cacheValue);
            return _redisDb.ListInsertBefore(cacheKey, pivotbytes, cacheValuebytes);
        }

        public long LInsertAfter<T>(String cacheKey, T pivot, T cacheValue)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var pivotbytes = _serializer.Serialize(pivot);
            var cacheValuebytes = _serializer.Serialize(cacheValue);
            return _redisDb.ListInsertAfter(cacheKey, pivotbytes, cacheValuebytes);
        }

        public long RPushX<T>(String cacheKey, T cacheValue)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var bytes = _serializer.Serialize(cacheValue);
            return _redisDb.ListRightPush(cacheKey, bytes);
        }

        public long RPush<T>(String cacheKey, IList<T> cacheValues)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNullAndCountGTZero(cacheValues, nameof(cacheValues));

            var list = new List<RedisValue>();

            foreach (var item in cacheValues)
            {
                list.Add(_serializer.Serialize(item));
            }

            var len = _redisDb.ListRightPush(cacheKey, list.ToArray());
            return len;
        }

        public T RPop<T>(String cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var bytes = _redisDb.ListRightPop(cacheKey);
            return _serializer.Deserialize<T>(bytes);
        }

        public async Task<T> LIndexAsync<T>(String cacheKey, long index)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var bytes = await _redisDb.ListGetByIndexAsync(cacheKey, index);
            return _serializer.Deserialize<T>(bytes);
        }

        public async Task<long> LLenAsync(String cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            return await _redisDb.ListLengthAsync(cacheKey);
        }

        public async Task<T> LPopAsync<T>(String cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var bytes = await _redisDb.ListLeftPopAsync(cacheKey);
            return _serializer.Deserialize<T>(bytes);
        }

        public async Task<long> LPushAsync<T>(String cacheKey, IList<T> cacheValues)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNullAndCountGTZero(cacheValues, nameof(cacheValues));

            var list = new List<RedisValue>();

            foreach (var item in cacheValues)
            {
                list.Add(_serializer.Serialize(item));
            }

            var len = await _redisDb.ListLeftPushAsync(cacheKey, list.ToArray());
            return len;
        }

        public async Task<List<T>> LRangeAsync<T>(String cacheKey, long start, long stop)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var list = new List<T>();

            var bytes = await _redisDb.ListRangeAsync(cacheKey, start, stop);

            foreach (var item in bytes)
            {
                list.Add(_serializer.Deserialize<T>(item));
            }

            return list;
        }

        public async Task<long> LRemAsync<T>(String cacheKey, long count, T cacheValue)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var bytes = _serializer.Serialize(cacheValue);
            return await _redisDb.ListRemoveAsync(cacheKey, bytes, count);
        }

        public async Task<bool> LSetAsync<T>(String cacheKey, long index, T cacheValue)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var bytes = _serializer.Serialize(cacheValue);
            await _redisDb.ListSetByIndexAsync(cacheKey, index, bytes);
            return true;
        }

        public async Task<bool> LTrimAsync(String cacheKey, long start, long stop)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            await _redisDb.ListTrimAsync(cacheKey, start, stop);
            return true;
        }

        public Task<long> LPushXAsync<T>(String cacheKey, T cacheValue)
        {
            //ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            //var bytes = _serializer.Serialize(cacheValue);
            //return await _cache.ListLeftPushAsync(cacheKey, bytes);
            throw new NotImplementedException();
        }

        public async Task<long> LInsertBeforeAsync<T>(String cacheKey, T pivot, T cacheValue)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var pivotbytes = _serializer.Serialize(pivot);
            var cacheValuebytes = _serializer.Serialize(cacheValue);
            return await _redisDb.ListInsertBeforeAsync(cacheKey, pivotbytes, cacheValuebytes);
        }

        public async Task<long> LInsertAfterAsync<T>(String cacheKey, T pivot, T cacheValue)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var pivotbytes = _serializer.Serialize(pivot);
            var cacheValuebytes = _serializer.Serialize(cacheValue);
            return await _redisDb.ListInsertAfterAsync(cacheKey, pivotbytes, cacheValuebytes);
        }

        public Task<long> RPushXAsync<T>(String cacheKey, T cacheValue)
        {
            //ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            //var bytes = _serializer.Serialize(cacheValue);
            //return await _cache.ListRightPushAsync(cacheKey, bytes);
            throw new NotImplementedException();
        }

        public async Task<long> RPushAsync<T>(String cacheKey, IList<T> cacheValues)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNullAndCountGTZero(cacheValues, nameof(cacheValues));

            var list = new List<RedisValue>();

            foreach (var item in cacheValues)
            {
                list.Add(_serializer.Serialize(item));
            }

            var len = await _redisDb.ListRightPushAsync(cacheKey, list.ToArray());
            return len;
        }

        public async Task<T> RPopAsync<T>(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var bytes = await _redisDb.ListRightPopAsync(cacheKey);
            return _serializer.Deserialize<T>(bytes);
        }
    }
}
