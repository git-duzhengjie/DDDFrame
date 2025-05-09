﻿
using System;

namespace Infra.Cache.Interceptor
{
    /// <summary>
    /// 缓存拦截器属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class CachingInterceptorAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets a value indicating whether is hybrid provider.
        /// </summary>
        /// <value><c>true</c> if is hybrid provider; otherwise, <c>false</c>.</value>
        //public bool IsHybridProvider { get; set; } = false;

        /// <summary>
        /// Gets or sets the cache key prefix.
        /// </summary>
        /// <value>The cache key prefix.</value>
        public string CacheKeyPrefix { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the cache provider name. The default value is Louge.Infra.CachingInterceptorOptions.CacheProviderName"/>
        /// </summary>
        /// <value>The cache key prefix.</value>
        //public string CacheProviderName { get; set; }

        /// <summary>
        ///  Prevent cache provider errors from affecting business
        /// </summary>
        /// <value>The cache key prefix.</value>
        public bool IsHighAvailability { get; set; } = true;

        /// <summary>
        /// The cache keys
        /// </summary>
        public string CacheKey { get; set; } = string.Empty;
    }
}
