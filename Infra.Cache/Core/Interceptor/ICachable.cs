
using System;

namespace Louge.Infra.Core.Interceptor
{
    /// <summary>
    /// Cachable.
    /// </summary>
    public interface ICachable
    {
        /// <summary>
        /// Gets the cache key.
        /// </summary>
        /// <value>The cache key.</value>
        String CacheKey { get; }
    }
}
