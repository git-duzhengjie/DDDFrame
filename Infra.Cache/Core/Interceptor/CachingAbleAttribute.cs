
using System;

namespace Infra.Cache.Interceptor
{
    /// <summary>
    /// Infra.Cache able attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class CachingAbleAttribute : CachingInterceptorAttribute
    {
        /// <summary>
        /// Gets or sets the expiration. The default value is 30 second.
        /// </summary>
        /// <value>The expiration.</value>
        public Int32 Expiration { get; set; } = 30;
    }
}
