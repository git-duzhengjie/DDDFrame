using Infra.Core.Abstract;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 服务集合扩展 - 服务信息
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceInfo GetServiceInfo(this IServiceCollection services)
        {
            return services.GetSingletonInstance<IServiceInfo>();
        }
    }
}
