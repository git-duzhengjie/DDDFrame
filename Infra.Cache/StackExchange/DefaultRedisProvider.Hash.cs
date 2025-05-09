﻿
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
        public bool HMSet(string cacheKey, Dictionary<string, string> vals, TimeSpan? expiration = null)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            if (expiration.HasValue)
            {
                var list = new List<HashEntry>();

                foreach (var item in vals)
                {
                    list.Add(new HashEntry(item.Key, item.Value));
                }

                redisDb.HashSet(cacheKey, list.ToArray());

                var flag = redisDb.KeyExpire(cacheKey, expiration);

                return flag;
            }
            else
            {
                var list = new List<HashEntry>();

                foreach (var item in vals)
                {
                    list.Add(new HashEntry(item.Key, item.Value));
                }

                redisDb.HashSet(cacheKey, list.ToArray());

                return true;
            }
        }

        public bool HSet(string cacheKey, string field, string cacheValue)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNullOrWhiteSpace(field, nameof(field));

            return redisDb.HashSet(cacheKey, field, cacheValue);
        }

        public bool HExists(string cacheKey, string field)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNullOrWhiteSpace(field, nameof(field));

            return redisDb.HashExists(cacheKey, field);
        }

        public long HDel(string cacheKey, IList<string> fields = null)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            if (fields != null && fields.Any())
            {
                return redisDb.HashDelete(cacheKey, fields.Select(x => (RedisValue)x).ToArray());
            }
            else
            {
                var flag = redisDb.KeyDelete(cacheKey);
                return flag ? 1 : 0;
            }
        }

        public string HGet(string cacheKey, string field)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNullOrWhiteSpace(field, nameof(field));

            var res = redisDb.HashGet(cacheKey, field);
            return res;
        }

        public Dictionary<string, string> HGetAll(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var dict = new Dictionary<string, string>();

            var vals = redisDb.HashGetAll(cacheKey);

            foreach (var item in vals)
            {
                if (!dict.ContainsKey(item.Name)) dict.Add(item.Name, item.Value);
            }

            return dict;
        }

        public long HIncrBy(string cacheKey, string field, long val = 1)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNullOrWhiteSpace(field, nameof(field));

            return redisDb.HashIncrement(cacheKey, field, val);
        }

        public List<string> HKeys(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var keys = redisDb.HashKeys(cacheKey);
            return keys.Select(x => x.ToString()).ToList();
        }

        public long HLen(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            return redisDb.HashLength(cacheKey);
        }

        public List<string> HVals(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            return redisDb.HashValues(cacheKey).Select(x => x.ToString()).ToList();
        }

        public Dictionary<string, string> HMGet(string cacheKey, IList<string> fields)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNullAndCountGTZero(fields, nameof(fields));

            var dict = new Dictionary<string, string>();

            var list = redisDb.HashGet(cacheKey, fields.Select(x => (RedisValue)x).ToArray());

            for (int i = 0; i < fields.Count(); i++)
            {
                if (!dict.ContainsKey(fields[i]))
                {
                    dict.Add(fields[i], list[i]);
                }
            }

            return dict;
        }

        public async Task<bool> HMSetAsync(string cacheKey, Dictionary<string, string> vals, TimeSpan? expiration = null)
        {
            if (expiration.HasValue)
            {
                var list = new List<HashEntry>();

                foreach (var item in vals)
                {
                    list.Add(new HashEntry(item.Key, item.Value));
                }

                await redisDb.HashSetAsync(cacheKey, list.ToArray());

                var flag = await redisDb.KeyExpireAsync(cacheKey, expiration.Value);

                return flag;
            }
            else
            {
                var list = new List<HashEntry>();

                foreach (var item in vals)
                {
                    list.Add(new HashEntry(item.Key, item.Value));
                }

                await redisDb.HashSetAsync(cacheKey, list.ToArray());
                return true;
            }
        }

        public async Task<bool> HSetAsync(string cacheKey, string field, string cacheValue)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNullOrWhiteSpace(field, nameof(field));

            return await redisDb.HashSetAsync(cacheKey, field, cacheValue);
        }

        public async Task<bool> HExistsAsync(string cacheKey, string field)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNullOrWhiteSpace(field, nameof(field));

            return await redisDb.HashExistsAsync(cacheKey, field);
        }

        public async Task<long> HDelAsync(string cacheKey, IList<string> fields)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            if (fields != null && fields.Any())
            {
                return await redisDb.HashDeleteAsync(cacheKey, fields.Select(x => (RedisValue)x).ToArray());
            }
            else
            {
                var flag = await redisDb.KeyDeleteAsync(cacheKey);
                return flag ? 1 : 0;
            }
        }

        public async Task<string> HGetAsync(string cacheKey, string field)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNullOrWhiteSpace(field, nameof(field));

            var res = await redisDb.HashGetAsync(cacheKey, field);
            return res;
        }

        public async Task<Dictionary<string, string>> HGetAllAsync(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var dict = new Dictionary<string, string>();

            var vals = await redisDb.HashGetAllAsync(cacheKey);

            foreach (var item in vals)
            {
                if (!dict.ContainsKey(item.Name)) dict.Add(item.Name, item.Value);
            }

            return dict;
        }

        public async Task<long> HIncrByAsync(string cacheKey, string field, long val = 1)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNullOrWhiteSpace(field, nameof(field));

            return await redisDb.HashIncrementAsync(cacheKey, field, val);
        }

        public async Task<List<string>> HKeysAsync(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var keys = await redisDb.HashKeysAsync(cacheKey);
            return keys.Select(x => x.ToString()).ToList();
        }

        public async Task<long> HLenAsync(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            return await redisDb.HashLengthAsync(cacheKey);
        }

        public async Task<List<string>> HValsAsync(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            return (await redisDb.HashValuesAsync(cacheKey)).Select(x => x.ToString()).ToList();
        }

        public async Task<Dictionary<string, string>> HMGetAsync(string cacheKey, IList<string> fields)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNullAndCountGTZero(fields, nameof(fields));

            var dict = new Dictionary<string, string>();

            var res = await redisDb.HashGetAsync(cacheKey, fields.Select(x => (RedisValue)x).ToArray());

            for (int i = 0; i < fields.Count(); i++)
            {
                if (!dict.ContainsKey(fields[i]))
                {
                    dict.Add(fields[i], res[i]);
                }
            }

            return dict;
        }
    }
}
