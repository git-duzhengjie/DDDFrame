﻿
using Infra.Cache.Core;
using Microsoft.Extensions.Logging;
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
    public partial class DefaultRedisProvider : AbstracCacheProvider, ICacheProvider
    {
        /// <summary>
        /// Gets the specified cacheKey async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="cacheKey">Cache key.</param>
        /// <param name="type">object Type.</param>
        protected override async Task<object> BaseGetAsync(string cacheKey, Type type)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            if (!cacheOptions.PenetrationSetting.Disable)
            {
                var exists = await redisDb.BloomExistsAsync(cacheOptions.PenetrationSetting.BloomFilterSetting.Name, cacheKey);
                if (!exists)
                {
                    if (cacheOptions.EnableLogging)
                        logger?.LogInformation($"Cache Penetrated : cachekey = {cacheKey}");
                    return null;
                }
            }

            var result = await redisDb.StringGetAsync(cacheKey);
            if (!result.IsNull)
            {
                _cacheStats.OnHit();

                if (cacheOptions.EnableLogging)
                    logger?.LogInformation($"Cache Hit : cachekey = {cacheKey}");

                var value = serializer.Deserialize(result, type);
                return value;
            }
            else
            {
                _cacheStats.OnMiss();

                if (cacheOptions.EnableLogging)
                    logger?.LogInformation($"Cache Missed : cachekey = {cacheKey}");

                return null;
            }
        }

        /// <summary>
        /// Gets the specified cacheKey, dataRetriever and expiration async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="cacheKey">Cache key.</param>
        /// <param name="dataRetriever">Data retriever.</param>
        /// <param name="expiration">Expiration.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected override async Task<CacheValue<T>> BaseGetAsync<T>(string cacheKey, Func<Task<T>> dataRetriever, TimeSpan expiration)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNegativeOrZero(expiration, nameof(expiration));

            if (!cacheOptions.PenetrationSetting.Disable)
            {
                var exists = await redisDb.BloomExistsAsync(cacheOptions.PenetrationSetting.BloomFilterSetting.Name, cacheKey);
                if (!exists)
                {
                    if (cacheOptions.EnableLogging)
                        logger?.LogInformation($"Cache Penetrated : cachekey = {cacheKey}");
                    return CacheValue<T>.NoValue;
                }
            }

            var result = await redisDb.StringGetAsync(cacheKey);
            if (!result.IsNull)
            {
                _cacheStats.OnHit();

                if (cacheOptions.EnableLogging)
                    logger?.LogInformation($"Cache Hit : cachekey = {cacheKey}");

                var value = serializer.Deserialize<T>(result);
                return new CacheValue<T>(value, true);
            }

            _cacheStats.OnMiss();

            if (cacheOptions.EnableLogging)
                logger?.LogInformation($"Cache Missed : cachekey = {cacheKey}");

            var flag = await redisDb.LockAsync(cacheKey, cacheOptions.LockMs / 1000);

            if (!flag.Success)
            {
                await Task.Delay(cacheOptions.SleepMs);
                return await GetAsync(cacheKey, dataRetriever, expiration);
            }

            try
            {
                var item = await dataRetriever();
                if (item != null)
                {
                    await SetAsync(cacheKey, item, expiration);
                    return new CacheValue<T>(item, true);
                }
                else
                {
                    return CacheValue<T>.NoValue;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                //remove mutex key
                await redisDb.SafedUnLockAsync(cacheKey, flag.LockValue);
            }
        }

        /// <summary>
        /// Gets the specified cacheKey async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="cacheKey">Cache key.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected override async Task<CacheValue<T>> BaseGetAsync<T>(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            if (!cacheOptions.PenetrationSetting.Disable)
            {
                var exists = await redisDb.BloomExistsAsync(cacheOptions.PenetrationSetting.BloomFilterSetting.Name, cacheKey);
                if (!exists)
                {
                    if (cacheOptions.EnableLogging)
                        logger?.LogInformation($"Cache Penetrated : cachekey = {cacheKey}");
                    return CacheValue<T>.NoValue;
                }
            }

            var result = await redisDb.StringGetAsync(cacheKey);
            if (!result.IsNull)
            {
                _cacheStats.OnHit();

                if (cacheOptions.EnableLogging)
                    logger?.LogInformation($"Cache Hit : cachekey = {cacheKey}");

                var value = serializer.Deserialize<T>(result);
                return new CacheValue<T>(value, true);
            }
            else
            {
                _cacheStats.OnMiss();

                if (cacheOptions.EnableLogging)
                    logger?.LogInformation($"Cache Missed : cachekey = {cacheKey}");

                return CacheValue<T>.NoValue;
            }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <returns>The count.</returns>
        /// <param name="prefix">Prefix.</param>
        protected override Task<int> BaseGetCountAsync(string prefix = "")
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                var allCount = 0;

                foreach (var server in servers)
                    allCount += (int)server.DatabaseSize(redisDb.Database);

                return Task.FromResult(allCount);
            }

            return Task.FromResult(this.SearchRedisKeys(this.HandlePrefix(prefix)).Length);
        }

        /// <summary>
        /// Removes the specified cacheKey async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="cacheKey">Cache key.</param>
        protected override async Task BaseRemoveAsync(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            await redisDb.KeyDeleteAsync(cacheKey);
        }

        /// <summary>
        /// Sets the specified cacheKey, cacheValue and expiration async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="cacheKey">Cache key.</param>
        /// <param name="cacheValue">Cache value.</param>
        /// <param name="expiration">Expiration.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected override async Task BaseSetAsync<T>(string cacheKey, T cacheValue, TimeSpan expiration)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNull(cacheValue, nameof(cacheValue));
            ArgumentCheck.NotNegativeOrZero(expiration, nameof(expiration));

            if (cacheOptions.MaxRdSecond > 0)
            {
                var addSec = new Random().Next(1, cacheOptions.MaxRdSecond);
                expiration = expiration.Add(TimeSpan.FromSeconds(addSec));
            }

            await redisDb.StringSetAsync(
                    cacheKey,
                    serializer.Serialize(cacheValue),
                    expiration);
        }

        /// <summary>
        /// Existses the specified cacheKey async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="cacheKey">Cache key.</param>
        protected override async Task<bool> BaseExistsAsync(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            return await redisDb.KeyExistsAsync(cacheKey);
        }

        /// <summary>
        /// Removes cached item by cachekey's prefix async.
        /// </summary>
        /// <param name="prefix">Prefix of CacheKey.</param>
        protected override async Task BaseRemoveByPrefixAsync(string prefix)
        {
            ArgumentCheck.NotNullOrWhiteSpace(prefix, nameof(prefix));

            prefix = this.HandlePrefix(prefix);

            if (cacheOptions.EnableLogging)
                logger?.LogInformation($"RemoveByPrefixAsync : prefix = {prefix}");

            var redisKeys = this.SearchRedisKeys(prefix);

            await redisDb.KeyDeleteAsync(redisKeys);
        }

        /// <summary>
        /// Sets all async.
        /// </summary>
        /// <returns>The all async.</returns>
        /// <param name="values">Values.</param>
        /// <param name="expiration">Expiration.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected override async Task BaseSetAllAsync<T>(IDictionary<string, T> values, TimeSpan expiration)
        {
            ArgumentCheck.NotNegativeOrZero(expiration, nameof(expiration));
            ArgumentCheck.NotNullAndCountGTZero(values, nameof(values));

            var tasks = new List<Task>();

            foreach (var item in values)
                tasks.Add(SetAsync(item.Key, item.Value, expiration));

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Gets all async.
        /// </summary>
        /// <returns>The all async.</returns>
        /// <param name="cacheKeys">Cache keys.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected override async Task<IDictionary<string, CacheValue<T>>> BaseGetAllAsync<T>(IEnumerable<string> cacheKeys)
        {
            ArgumentCheck.NotNullAndCountGTZero(cacheKeys, nameof(cacheKeys));

            var keyArray = cacheKeys.ToArray();
            var values = await redisDb.StringGetAsync(keyArray.Select(k => (RedisKey)k).ToArray());

            var result = new Dictionary<string, CacheValue<T>>();
            for (int i = 0; i < keyArray.Length; i++)
            {
                var cachedValue = values[i];
                if (!cachedValue.IsNull)
                    result.Add(keyArray[i], new CacheValue<T>(serializer.Deserialize<T>(cachedValue), true));
                else
                    result.Add(keyArray[i], CacheValue<T>.NoValue);
            }

            return result;
        }

        /// <summary>
        /// Gets the by prefix async.
        /// </summary>
        /// <returns>The by prefix async.</returns>
        /// <param name="prefix">Prefix.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected override async Task<IDictionary<string, CacheValue<T>>> BaseGetByPrefixAsync<T>(string prefix)
        {
            ArgumentCheck.NotNullOrWhiteSpace(prefix, nameof(prefix));

            prefix = this.HandlePrefix(prefix);

            var redisKeys = this.SearchRedisKeys(prefix);

            var values = (await redisDb.StringGetAsync(redisKeys)).ToArray();

            var result = new Dictionary<string, CacheValue<T>>();
            for (int i = 0; i < redisKeys.Length; i++)
            {
                var cachedValue = values[i];
                if (!cachedValue.IsNull)
                    result.Add(redisKeys[i], new CacheValue<T>(serializer.Deserialize<T>(cachedValue), true));
                else
                    result.Add(redisKeys[i], CacheValue<T>.NoValue);
            }

            return result;
        }

        /// <summary>
        /// Removes all async.
        /// </summary>
        /// <returns>The all async.</returns>
        /// <param name="cacheKeys">Cache keys.</param>
        protected override async Task BaseRemoveAllAsync(IEnumerable<string> cacheKeys)
        {
            ArgumentCheck.NotNullAndCountGTZero(cacheKeys, nameof(cacheKeys));

            var redisKeys = cacheKeys.Where(k => !string.IsNullOrEmpty(k)).Select(k => (RedisKey)k).ToArray();
            if (redisKeys.Length > 0)
                await redisDb.KeyDeleteAsync(redisKeys);
        }

        /// <summary>
        /// Flush All Cached Item async.
        /// </summary>
        /// <returns>The async.</returns>
        protected override async Task BaseFlushAsync()
        {
            if (cacheOptions.EnableLogging)
                logger?.LogInformation("Redis -- FlushAsync");

            var tasks = new List<Task>();

            foreach (var server in servers)
            {
                tasks.Add(server.FlushDatabaseAsync(redisDb.Database));
            }

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Tries the set async.
        /// </summary>
        /// <returns>The set async.</returns>
        /// <param name="cacheKey">Cache key.</param>
        /// <param name="cacheValue">Cache value.</param>
        /// <param name="expiration">Expiration.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected override Task<bool> BaseTrySetAsync<T>(string cacheKey, T cacheValue, TimeSpan expiration)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNull(cacheValue, nameof(cacheValue));
            ArgumentCheck.NotNegativeOrZero(expiration, nameof(expiration));

            if (cacheOptions.MaxRdSecond > 0)
            {
                var addSec = new Random().Next(1, cacheOptions.MaxRdSecond);
                expiration = expiration.Add(TimeSpan.FromSeconds(addSec));
            }

            return redisDb.StringSetAsync(
                cacheKey,
                serializer.Serialize(cacheValue),
                expiration,
                When.NotExists
                );
        }

        /// <summary>
        /// Get the expiration of cache key
        /// </summary>
        /// <param name="cacheKey">cache key</param>
        /// <returns>expiration</returns>
        protected override async Task<TimeSpan> BaseGetExpirationAsync(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var timeSpan = await redisDb.KeyTimeToLiveAsync(cacheKey);
            return timeSpan.HasValue ? timeSpan.Value : TimeSpan.Zero;
        }

        /// <summary>
        /// Get the expiration of cache key
        /// </summary>
        /// <param name="cacheKey">cache key</param>
        /// <returns>expiration</returns>
        protected override async Task BaseKeyExpireAsync(IEnumerable<string> cacheKeys, int seconds)
        {
            ArgumentCheck.NotNullAndCountGTZero(cacheKeys, nameof(cacheKeys));

            await redisDb.KeyExpireAsync(cacheKeys, seconds);
        }
    }
}
