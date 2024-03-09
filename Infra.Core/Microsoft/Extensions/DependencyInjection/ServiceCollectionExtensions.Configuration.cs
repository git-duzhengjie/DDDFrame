
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 服务集合扩展 - 配置
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection ReplaceConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            return services.Replace(ServiceDescriptor.Singleton<IConfiguration>(configuration));
        }

        public static IConfiguration GetConfiguration(this IServiceCollection services)
        {
            //var configfileName = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            //var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            var hostBuilderContext = services.GetSingletonInstanceOrNull<HostBuilderContext>();
            if (hostBuilderContext?.Configuration != null)
            {
                //var configuration = hostBuilderContext.Configuration;
                return hostBuilderContext.Configuration as IConfigurationRoot;
            }

            return services.GetSingletonInstance<IConfiguration>();

        }
    }
}
