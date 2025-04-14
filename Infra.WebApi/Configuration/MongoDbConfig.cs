
using System;

namespace Infra.WebApi.Configuration
{
    /// <summary>
    /// MongoDb配置
    /// </summary>
    public class MongoDbConfig
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        public string Database { get; set; }
    }
}
