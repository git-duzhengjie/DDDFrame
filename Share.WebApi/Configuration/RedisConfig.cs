using System;

namespace Infra.WebApi.Configuration
{
    /// <summary>
    /// Redis配置
    /// </summary>
    public class RedisConfig
    {
        /// <summary>
        /// 防止cahce雪崩配置,cahce保存时，过期时间会加上最大不超过MaxRdSecond的一个随机数。
        /// </summary>
        public Int32 MaxRdSecond { get; set; }
        
        /// <summary>
        /// 启用日志
        /// </summary>
        public Boolean EnableLogging { get; set; }

        /// <summary>
        /// 防止cache击穿配置，LockMs是获取到分布式锁的锁定时间，SleepMs没有获取到分布式锁的休眠时间，具体实现请参考源码。
        /// </summary>
        public Int32 LockMs { get; set; }

        public Int32 SleepMs { get; set; }

        public Dbconfig dbconfig { get; set; }
    }

    /// <summary>
    /// 数据库配置
    /// </summary>
    public class Dbconfig
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 仅可读
        /// </summary>
        public Boolean ReadOnly { get; set; }
    }
}
