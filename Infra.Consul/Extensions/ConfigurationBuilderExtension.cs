

using Infra.Consul.Configuration;

namespace Microsoft.Extensions.Configuration
{
    public static class ConfigurationBuilderExtension
    {
        /// <summary>
        /// 添加Consul配置
        /// </summary>
        /// <param name="configurationBuilder"></param>
        /// <param name="config"></param>
        /// <param name="reloadOnChanges"></param>
        /// <returns></returns>
        public static IConfigurationBuilder AddConsulConfiguration(this IConfigurationBuilder configurationBuilder, ConsulConfig config, bool reloadOnChanges = false)
        => configurationBuilder.Add(new DefaultConsulConfigurationSource(config, reloadOnChanges));
    }
}
