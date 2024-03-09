
using Infra.Core;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 服务集合扩展 - 注入
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        public static IDependencyInjection GetWebApiRegistrar(this IServiceCollection services)
        {

            return services.GetSingletonInstance<IDependencyInjection>();
        }
    }
}
