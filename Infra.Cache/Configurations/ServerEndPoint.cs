
using Infra.Cache.Core;
using System;

namespace Infra.Cache.Configurations
{
    /// <summary>
    /// 服务终端
    /// </summary>
    public class ServerEndPoint
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="host">主机</param>
        /// <param name="port">端口</param>
        public ServerEndPoint(String host, Int32 port)
        {
            ArgumentCheck.NotNullOrWhiteSpace(host, nameof(host));

            Host = host;
            Port = port;
        }

        /// <summary>
        /// 默认构造
        /// </summary>
        public ServerEndPoint()
        {
        }

        /// <summary>
        /// 端口
        /// </summary>
        public Int32 Port { get; set; }

        /// <summary>
        /// 主机
        /// </summary>
        public String Host { get; set; }
    }
}
