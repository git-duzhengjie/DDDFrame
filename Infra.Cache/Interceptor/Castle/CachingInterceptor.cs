using Castle.DynamicProxy;

namespace Infra.Cache.Interceptor.Castle
{
    /// <summary>
    /// caching interceptor
    /// </summary>
    public class CachingInterceptor : IInterceptor
    {
        private readonly CachingAsyncInterceptor _cachingAsyncInterceptor;

        public CachingInterceptor(CachingAsyncInterceptor cachingAsyncInterceptor)
        {
            _cachingAsyncInterceptor = cachingAsyncInterceptor;
        }

        public void Intercept(IInvocation invocation)
        {
            _cachingAsyncInterceptor.ToInterceptor().Intercept(invocation);
        }
    }
}
