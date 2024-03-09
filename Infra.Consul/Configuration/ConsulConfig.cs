
using System;

namespace Infra.Consul.Configuration
{
    /// <summary>
    /// Consul配置
    /// </summary>
    public class ConsulConfig
    {
        /// <summary>
        /// Url地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// 健康检测地址
        /// </summary>
        public string HealthCheckUrl { get; set; }

        /// <summary>
        /// 健康检测时间间隔（单位：秒）
        /// </summary>
        public Int32 HealthCheckIntervalInSecond { get; set; }

        public string[] ServerTags { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Port { get; set; }

        public Int32 DeregisterCriticalServiceAfter { get; set; }
        
        /// <summary>
        /// 超时
        /// </summary>
        public Int32 Timeout { get; set; }
    }
}
