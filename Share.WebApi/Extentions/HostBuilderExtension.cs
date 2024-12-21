
using Infra.Consul.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Infra.WebApi;
using NLog.Web;
using Microsoft.AspNetCore.Builder;
using Infra.Core.Abstract;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// 主机创建扩展
    /// </summary>
    public static class HostBuilderExtension
    {
        public static WebApplicationBuilder UseDefault<TRegistrar>(this WebApplicationBuilder hostBuilder, string[] args)
            where TRegistrar : IDependencyInjection
        {
            if (hostBuilder is null) throw new ArgumentNullException(nameof(hostBuilder));
            hostBuilder.Configuration.AddJsonFile($"{AppContext.BaseDirectory}/appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true, true);
            hostBuilder.Configuration.AddConsulConfiguration(hostBuilder.Configuration.GetConsulSection().Get<ConsulConfig>(),true);
            var services=hostBuilder.Services;
            var startupAssembly = typeof(TRegistrar).Assembly;
            var serviceInfoInstance = ServiceInfo.GetInstance(startupAssembly);
            services.Add(ServiceDescriptor.Singleton(typeof(IServiceInfo), serviceInfoInstance));
            var registrarInstance = Activator.CreateInstance(typeof(TRegistrar), services) as IDependencyInjection;
            registrarInstance.AddWebApiDefault();
            services.Add(ServiceDescriptor.Singleton(typeof(IDependencyInjection), registrarInstance));
            return hostBuilder;
        }
    }
}
