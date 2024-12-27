using Infra.Consul.Registrar;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 服务集合扩展
    /// </summary>
    public static class ServiceCollectionExtension
    {

        public static IServiceCollection AddInfraConsul(this IServiceCollection services)
        {
            if (services.HasRegistered(nameof(AddInfraConsul)))
                return services;
            services.AddTransient<IConsulRegister, ConsulRegister>();
            return services;
        }
    }
}
