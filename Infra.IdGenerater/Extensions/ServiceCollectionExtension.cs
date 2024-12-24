using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infra.Cache.Extensions;
using Infra.IdGenerater.Yitter;
namespace Infra.IdGenerater.Extensions
{
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// 注册Id生成器服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="redisSection"></param>
        /// <returns></returns>
        public static IServiceCollection AddInfraYitterIdGenerater(this IServiceCollection services, IConfigurationSection redisSection)
        {
            if (services.HasRegistered(nameof(AddInfraYitterIdGenerater)))
                return services;

            return services
                .AddInfraCaching(redisSection)
                .AddSingleton<WorkerNode>()
                .AddHostedService<WorkerNodeHostedService>();
        }
    }
}
