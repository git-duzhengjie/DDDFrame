

using Microsoft.Extensions.Configuration;

namespace Infra.Consul.Configuration
{
    /// <summary>
    /// 默认Consul配置源
    /// </summary>
    public class DefaultConsulConfigurationSource : IConfigurationSource
    {
        #region <变量>

        private readonly ConsulConfig _config;
        private readonly Boolean _reloadOnChanges;

        #endregion <变量>

        #region <构造方法和析构方法>

        public DefaultConsulConfigurationSource(ConsulConfig config, Boolean reloadOnChanges)
        {
            _config = config;
            _reloadOnChanges = reloadOnChanges;
        }

        #endregion <构造方法和析构方法>

        #region <方法>

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new DefaultConsulConfigurationProvider(_config, _reloadOnChanges);
        }


        #endregion <方法>
    }
}
