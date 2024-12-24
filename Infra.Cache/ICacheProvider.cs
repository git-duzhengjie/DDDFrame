
using Infra.Cache.Configurations;
using Infra.Cache.Core;
using Infra.Cache.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infra.Cache
{
    /// <summary>
    /// 缓存提供者接口
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
        /// 名称
        /// </summary>
        String Name { get; }

        /// <summary>
        /// 配置
        /// </summary>
        CacheOptions CacheOptions { get; }

        /// <summary>
        /// 序列化
        /// </summary>
        ICachingSerializer Serializer { get; }

        /// <summary>
        /// Set the specified cacheKey, cacheValue and expiration.
        /// </summary>
        /// <param name="cacheKey">Cache key.</param>
        /// <param name="cacheValue">Cache value.</param>
        /// <param name="expiration">Expiration.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        void Set<T>(String cacheKey, T cacheValue, TimeSpan expiration);

        /// <summary>
        /// Sets the specified cacheKey, cacheValue and expiration async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="cacheKey">Cache key.</param>
        /// <param name="cacheValue">Cache value.</param>
        /// <param name="expiration">Expiration.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        Task SetAsync<T>(String cacheKey, T cacheValue, TimeSpan expiration);

        /// <summary>
        /// Get the specified cacheKey.
        /// </summary>
        /// <returns>The get.</returns>
        /// <param name="cacheKey">Cache key.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        CacheValue<T> Get<T>(String cacheKey);

        /// <summary>
        /// Get the specified cacheKey async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="cacheKey">Cache key.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        Task<CacheValue<T>> GetAsync<T>(String cacheKey);

        /// <summary>
        /// Remove the specified cacheKey.
        /// </summary>
        /// <param name="cacheKey">Cache key.</param>
        void Remove(String cacheKey);

        /// <summary>
        /// Remove the specified cacheKey async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="cacheKey">Cache key.</param>
        Task RemoveAsync(String cacheKey);

        /// <summary>
        /// Exists the specified cacheKey async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="cacheKey">Cache key.</param>
        Task<bool> ExistsAsync(String cacheKey);

        /// <summary>
        /// Exists the specified cacheKey.
        /// </summary>
        /// <returns>The exists.</returns>
        /// <param name="cacheKey">Cache key.</param>
        bool Exists(String cacheKey);

        /// <summary>
        /// Tries the set.
        /// </summary>
        /// <returns><c>true</c>, if set was tryed, <c>false</c> otherwise.</returns>
        /// <param name="cacheKey">Cache key.</param>
        /// <param name="cacheValue">Cache value.</param>
        /// <param name="expiration">Expiration.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        bool TrySet<T>(String cacheKey, T cacheValue, TimeSpan expiration);

        /// <summary>
        /// Tries the set async.
        /// </summary>
        /// <returns>The set async.</returns>
        /// <param name="cacheKey">Cache key.</param>
        /// <param name="cacheValue">Cache value.</param>
        /// <param name="expiration">Expiration.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        Task<bool> TrySetAsync<T>(String cacheKey, T cacheValue, TimeSpan expiration);

        /// <summary>
        /// Sets all.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="expiration">Expiration.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        void SetAll<T>(IDictionary<String, T> value, TimeSpan expiration);

        /// <summary>
        /// Sets all async.
        /// </summary>
        /// <returns>The all async.</returns>
        /// <param name="value">Value.</param>
        /// <param name="expiration">Expiration.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        Task SetAllAsync<T>(IDictionary<String, T> value, TimeSpan expiration);

        /// <summary>
        /// Removes all.
        /// </summary>
        /// <param name="cacheKeys">Cache keys.</param>
        void RemoveAll(IEnumerable<String> cacheKeys);

        /// <summary>
        /// Removes all async.
        /// </summary>
        /// <returns>The all async.</returns>
        /// <param name="cacheKeys">Cache keys.</param>
        Task RemoveAllAsync(IEnumerable<String> cacheKeys);

        /// <summary>
        /// Get the specified cacheKey, dataRetriever and expiration.
        /// </summary>
        /// <returns>The get.</returns>
        /// <param name="cacheKey">Cache key.</param>
        /// <param name="dataRetriever">Data retriever.</param>
        /// <param name="expiration">Expiration.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        CacheValue<T> Get<T>(String cacheKey, Func<T> dataRetriever, TimeSpan expiration);

        /// <summary>
        /// Gets the specified cacheKey, dataRetriever and expiration async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="cacheKey">Cache key.</param>
        /// <param name="dataRetriever">Data retriever.</param>
        /// <param name="expiration">Expiration.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        Task<CacheValue<T>> GetAsync<T>(String cacheKey, Func<Task<T>> dataRetriever, TimeSpan expiration);

        /// <summary>
        /// Removes cached item by cachekey's prefix.
        /// </summary>
        /// <param name="prefix">Prefix of CacheKey.</param>
        void RemoveByPrefix(String prefix);

        /// <summary>
        /// Removes cached item by cachekey's prefix async.
        /// </summary>
        /// <param name="prefix">Prefix of CacheKey.</param>
        Task RemoveByPrefixAsync(String prefix);

        /// <summary>
        /// Gets the specified cacheKey async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="cacheKey">Cache key.</param>
        /// <param name="type">object Type.</param>
        Task<object> GetAsync(String cacheKey, Type type);

        /// <summary>
        /// Set the keys  TTL
        /// </summary>
        /// <param name="cacheKeys">Cache keys.</param>
        /// <param name="seconds">Expiration .</param>
        /// <returns></returns>
        Task KeyExpireAsync(IEnumerable<String> cacheKeys, Int32 seconds);
    }
}
