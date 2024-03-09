
namespace Infra.Consul.Discover
{
    /// <summary>
    /// 服务创建者
    /// </summary>
    public class ServiceBuilder : IServiceBuilder
    {
        /// <summary>
        /// 服务提供者
        /// </summary>
        public IConsulServiceProvider ServiceProvider { get; init; }

        /// <summary>
        /// 服务名
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// Uri方案
        /// </summary>
        public string UriScheme { get; set; }

        /// <summary>
        /// 使用哪种策略
        /// </summary>
        public ILoadBalancer LoadBalancer { get; set; }

        /// <summary>
        /// 公共构造
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        public ServiceBuilder(IConsulServiceProvider serviceProvider) => ServiceProvider = serviceProvider;
    }

    /// <summary>
    /// 服务创建者接口
    /// </summary>
    public interface IServiceBuilder
    {
        /// <summary>
        /// 服务提供者
        /// </summary>
        IConsulServiceProvider ServiceProvider { get; init; }

        /// <summary>
        /// 服务名称
        /// </summary>
        string ServiceName { get; set; }

        /// <summary>
        /// Uri方案
        /// </summary>
        string UriScheme { get; set; }

        /// <summary>
        /// 使用哪种策略
        /// </summary>
        ILoadBalancer LoadBalancer { get; set; }
    }
}
