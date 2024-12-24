using Infra.Cache.Configurations;
using Infra.Cache.Core.Serialization;
using Infra.Cache.Interceptor.Castle;
using Infra.Cache.StackExchange;
using Louge.Infra.Core.Interceptor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace Infra.Cache.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddInfraCaching(this IServiceCollection services, 
            IConfigurationSection redisSection)
        {
            var cacheOptions = redisSection.Get<CacheOptions>();
            return AddInfraCaching(services, cacheOptions);
        }

        public static IServiceCollection AddInfraCaching(this IServiceCollection services, CacheOptions cacheOptions)
        {
            if (services.HasRegistered(nameof(AddInfraCaching)))
                return services;

            services.AddSingleton(cacheOptions);
            services.AddSingleton<IRedisDatabaseProvider, DefaultDatabaseProvider>();
            services.AddSingleton<ICachingKeyGenerator, DefaultCachingKeyGenerator>();
            services.AddSingleton<DefaultRedisProvider>();
            services.AddSingleton<IRedisProvider>(x => x.GetRequiredService<DefaultRedisProvider>());
            services.AddSingleton<IDistributedLocker>(x => x.GetRequiredService<DefaultRedisProvider>());
            services.AddSingleton<ICacheProvider>(x => x.GetRequiredService<DefaultRedisProvider>());
            services.AddScoped<CachingInterceptor>();
            services.AddScoped<CachingAsyncInterceptor>();

            var serviceType = typeof(ICachingSerializer);
            var implementations = serviceType.Assembly.ExportedTypes.Where(type => serviceType.IsAssignableFrom(type) && type.IsNotAbstractClass(true)).ToList();
            implementations.ForEach(implementationType => services.AddSingleton(serviceType, implementationType));

            return services;
        }
    }
}
