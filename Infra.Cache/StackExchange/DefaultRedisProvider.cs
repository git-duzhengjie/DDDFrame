﻿
using Infra.Cache.Configurations;
using Infra.Cache.Core;
using Infra.Cache.Core.Serialization;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infra.Cache.StackExchange
{
    /// <summary>
    /// Default redis caching provider.
    /// </summary>
    public partial class DefaultRedisProvider : AbstracCacheProvider, ICacheProvider
    {
        /// <summary>
        /// The cache.
        /// </summary>
        private readonly IDatabase redisDb;

        /// <summary>
        /// The servers.
        /// </summary>
        private readonly IEnumerable<IServer> servers;

        /// <summary>
        /// The db provider.
        /// </summary>
        private readonly IRedisDatabaseProvider dbProvider;

        /// <summary>
        /// The serializer.
        /// </summary>
        private readonly ICachingSerializer serializer;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// The options.
        /// </summary>
        private readonly CacheOptions cacheOptions;

        public override CacheOptions CacheOptions => cacheOptions;

        /// <summary>
        /// The cache stats.
        /// </summary>
        private readonly CacheStats _cacheStats;

        public override string Name
        { get { return CachingConstValue.StackExchange; } }

        public override string CachingProviderType
        { get { return nameof(DefaultRedisProvider); } }

        /// <summary>
        /// The serializer.
        /// </summary>
        public override ICachingSerializer Serializer
        { get { return serializer; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Infra.Cache.Redis.DefaultRedisCachingProvider"/> class.
        /// </summary>
        /// <param name="dbProviders">Db providers.</param>
        /// <param name="serializers">Serializers.</param>
        /// <param name="cacheOptions">CacheOptions.</param>
        /// <param name="loggerFactory">Logger factory.</param>
        public DefaultRedisProvider(
            IRedisDatabaseProvider dbProviders,
            IEnumerable<ICachingSerializer> serializers,
            CacheOptions cacheOptions,
            ILoggerFactory loggerFactory = null)
        {
            ArgumentCheck.NotNull(dbProviders, nameof(dbProviders));

            //this.ProviderName = nameof(DefaultRedisProvider);
            this.dbProvider = dbProviders;
            this.serializer = !string.IsNullOrWhiteSpace(cacheOptions.SerializerName)
                                       ? serializers.Single(x => x.Name.Equals(cacheOptions.SerializerName))
                                       : serializers.Single(x => x.Name.Equals(CachingConstValue.DefaultSerializerName));
            this.cacheOptions = cacheOptions;
            this.logger = loggerFactory?.CreateLogger<DefaultRedisProvider>();
            this.redisDb = dbProvider.GetDatabase();
            this.servers = dbProvider.GetServerList();
            this._cacheStats = new CacheStats();
        }

        /// <summary>
        /// Get the specified cacheKey, dataRetriever and expiration.
        /// </summary>
        /// <returns>The get.</returns>
        /// <param name="cacheKey">Cache key.</param>
        /// <param name="dataRetriever">Data retriever.</param>
        /// <param name="expiration">Expiration.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected override CacheValue<T> BaseGet<T>(string cacheKey, Func<T> dataRetriever, TimeSpan expiration)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNegativeOrZero(expiration, nameof(expiration));

            if (!cacheOptions.PenetrationSetting.Disable)
            {
                var exists = redisDb.BloomExistsAsync(cacheOptions.PenetrationSetting.BloomFilterSetting.Name, cacheKey).GetAwaiter().GetResult();
                if (!exists)
                {
                    if (cacheOptions.EnableLogging)
                        logger?.LogInformation($"Cache Penetrated : cachekey = {cacheKey}");
                    return CacheValue<T>.NoValue;
                }
            }

            var result = redisDb.StringGet(cacheKey);
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

            var flag = redisDb.Lock(cacheKey, cacheOptions.LockMs / 1000);
            if (!flag.Success)
            {
                System.Threading.Thread.Sleep(cacheOptions.SleepMs);
                return Get(cacheKey, dataRetriever, expiration);
            }

            try
            {
                var item = dataRetriever();
                if (item != null)
                {
                    Set(cacheKey, item, expiration);
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
                redisDb.SafedUnLock(cacheKey, flag.LockValue);
            }
        }

        /// <summary>
        /// Get the specified cacheKey.
        /// </summary>
        /// <returns>The get.</returns>
        /// <param name="cacheKey">Cache key.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected override CacheValue<T> BaseGet<T>(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            if (!cacheOptions.PenetrationSetting.Disable)
            {
                var exists = redisDb.BloomExistsAsync(cacheOptions.PenetrationSetting.BloomFilterSetting.Name, cacheKey).GetAwaiter().GetResult();
                if (!exists)
                {
                    if (cacheOptions.EnableLogging)
                        logger?.LogInformation($"Cache Penetrated : cachekey = {cacheKey}");
                    return CacheValue<T>.NoValue;
                }
            }

            var result = redisDb.StringGet(cacheKey);
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
        /// Remove the specified cacheKey.
        /// </summary>
        /// <returns>The remove.</returns>
        /// <param name="cacheKey">Cache key.</param>
        protected override void BaseRemove(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            redisDb.KeyDelete(cacheKey);
        }

        /// <summary>
        /// Set the specified cacheKey, cacheValue and expiration.
        /// </summary>
        /// <returns>The set.</returns>
        /// <param name="cacheKey">Cache key.</param>
        /// <param name="cacheValue">Cache value.</param>
        /// <param name="expiration">Expiration.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected override void BaseSet<T>(string cacheKey, T cacheValue, TimeSpan expiration)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNull(cacheValue, nameof(cacheValue));
            ArgumentCheck.NotNegativeOrZero(expiration, nameof(expiration));

            if (cacheOptions.MaxRdSecond > 0)
            {
                var addSec = new Random().Next(1, cacheOptions.MaxRdSecond);
                expiration = expiration.Add(TimeSpan.FromSeconds(addSec));
            }

            redisDb.StringSet(
                cacheKey,
                serializer.Serialize(cacheValue),
                expiration);
        }

        /// <summary>
        /// Exists the specified cacheKey.
        /// </summary>
        /// <returns>The exists.</returns>
        /// <param name="cacheKey">Cache key.</param>
        protected override bool BaseExists(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            return redisDb.KeyExists(cacheKey);
        }

        /// <summary>
        /// Removes cached item by cachekey's prefix.
        /// </summary>
        /// <param name="prefix">Prefix of CacheKey.</param>
        protected override void BaseRemoveByPrefix(string prefix)
        {
            ArgumentCheck.NotNullOrWhiteSpace(prefix, nameof(prefix));

            prefix = this.HandlePrefix(prefix);

            if (cacheOptions.EnableLogging)
                logger?.LogInformation($"RemoveByPrefix : prefix = {prefix}");

            var redisKeys = this.SearchRedisKeys(prefix);

            redisDb.KeyDelete(redisKeys);
        }

        /// <summary>
        /// Searchs the redis keys.
        /// </summary>
        /// <returns>The redis keys.</returns>
        /// <remarks>
        /// If your Redis Servers support command SCAN ,
        /// IServer.Keys will use command SCAN to find out the keys.
        /// Following
        /// https://github.com/StackExchange/StackExchange.Redis/blob/master/StackExchange.Redis/StackExchange/Redis/RedisServer.cs#L289
        /// </remarks>
        /// <param name="pattern">Pattern.</param>
        private RedisKey[] SearchRedisKeys(string pattern)
        {
            var keys = new List<RedisKey>();

            foreach (var server in servers)
                keys.AddRange(server.Keys(pattern: pattern, database: redisDb.Database, pageSize: 200));

            return keys.Distinct().ToArray();

            //var keys = new HashSet<RedisKey>();

            //int nextCursor = 0;
            //do
            //{
            //    RedisResult redisResult = _cache.Execute("SCAN", nextCursor.ToString(), "MATCH", pattern, "COUNT", "1000");
            //    var innerResult = (RedisResult[])redisResult;

            //    nextCursor = int.Parse((string)innerResult[0]);

            //    List<RedisKey> resultLines = ((RedisKey[])innerResult[1]).ToList();

            //    keys.UnionWith(resultLines);
            //}
            //while (nextCursor != 0);

            //return keys.ToArray();
        }

        /// <summary>
        /// Handles the prefix of CacheKey.
        /// </summary>
        /// <param name="prefix">Prefix of CacheKey.</param>
        /// <exception cref="ArgumentException"></exception>
        private string HandlePrefix(string prefix)
        {
            // Forbid
            if (prefix.Equals("*"))
                throw new ArgumentException("the prefix should not equal to *");

            // Don't start with *
            prefix = new System.Text.RegularExpressions.Regex("^\\*+").Replace(prefix, "");

            // End with *
            if (!prefix.EndsWith("*", StringComparison.OrdinalIgnoreCase))
                prefix = string.Concat(prefix, "*");

            return prefix;
        }

        /// <summary>
        /// Sets all.
        /// </summary>
        /// <param name="values">Values.</param>
        /// <param name="expiration">Expiration.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected override void BaseSetAll<T>(IDictionary<string, T> values, TimeSpan expiration)
        {
            ArgumentCheck.NotNegativeOrZero(expiration, nameof(expiration));
            ArgumentCheck.NotNullAndCountGTZero(values, nameof(values));

            var batch = redisDb.CreateBatch();

            foreach (var item in values)
                batch.StringSetAsync(item.Key, serializer.Serialize(item.Value), expiration);

            batch.Execute();
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>The all.</returns>
        /// <param name="cacheKeys">Cache keys.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected override IDictionary<string, CacheValue<T>> BaseGetAll<T>(IEnumerable<string> cacheKeys)
        {
            ArgumentCheck.NotNullAndCountGTZero(cacheKeys, nameof(cacheKeys));

            var keyArray = cacheKeys.ToArray();
            var values = redisDb.StringGet(keyArray.Select(k => (RedisKey)k).ToArray());

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
        /// Gets the by prefix.
        /// </summary>
        /// <returns>The by prefix.</returns>
        /// <param name="prefix">Prefix.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected override IDictionary<string, CacheValue<T>> BaseGetByPrefix<T>(string prefix)
        {
            ArgumentCheck.NotNullOrWhiteSpace(prefix, nameof(prefix));

            prefix = this.HandlePrefix(prefix);

            var redisKeys = this.SearchRedisKeys(prefix);

            var values = redisDb.StringGet(redisKeys).ToArray();

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
        /// Removes all.
        /// </summary>
        /// <param name="cacheKeys">Cache keys.</param>
        protected override void BaseRemoveAll(IEnumerable<string> cacheKeys)
        {
            ArgumentCheck.NotNullAndCountGTZero(cacheKeys, nameof(cacheKeys));

            var redisKeys = cacheKeys.Where(k => !string.IsNullOrEmpty(k)).Select(k => (RedisKey)k).ToArray();
            if (redisKeys.Length > 0)
                redisDb.KeyDelete(redisKeys);
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <returns>The count.</returns>
        /// <param name="prefix">Prefix.</param>
        protected override int BaseGetCount(string prefix = "")
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                var allCount = 0;

                foreach (var server in servers)
                    allCount += (int)server.DatabaseSize(redisDb.Database);

                return allCount;
            }

            return this.SearchRedisKeys(this.HandlePrefix(prefix)).Length;
        }

        /// <summary>
        /// Flush All Cached Item.
        /// </summary>
        protected override void BaseFlush()
        {
            if (cacheOptions.EnableLogging)
                logger?.LogInformation("Redis -- Flush");

            foreach (var server in servers)
            {
                server.FlushDatabase(redisDb.Database);
            }
        }

        /// <summary>
        /// Tries the set.
        /// </summary>
        /// <returns><c>true</c>, if set was tryed, <c>false</c> otherwise.</returns>
        /// <param name="cacheKey">Cache key.</param>
        /// <param name="cacheValue">Cache value.</param>
        /// <param name="expiration">Expiration.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected override bool BaseTrySet<T>(string cacheKey, T cacheValue, TimeSpan expiration)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));
            ArgumentCheck.NotNull(cacheValue, nameof(cacheValue));
            ArgumentCheck.NotNegativeOrZero(expiration, nameof(expiration));

            if (cacheOptions.MaxRdSecond > 0)
            {
                var addSec = new Random().Next(1, cacheOptions.MaxRdSecond);
                expiration = expiration.Add(TimeSpan.FromSeconds(addSec));
            }

            return redisDb.StringSet(
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
        protected override TimeSpan BaseGetExpiration(string cacheKey)
        {
            ArgumentCheck.NotNullOrWhiteSpace(cacheKey, nameof(cacheKey));

            var timeSpan = redisDb.KeyTimeToLive(cacheKey);
            return timeSpan.HasValue ? timeSpan.Value : TimeSpan.Zero;
        }
    }
}
