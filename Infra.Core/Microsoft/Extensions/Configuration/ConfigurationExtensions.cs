
using Microsoft.Extensions.Configuration;
using System;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// 配置扩展类
    /// </summary>
    public static partial class ConfigurationExtensions
    {
        /// <summary>
        /// 服务注册类型
        /// </summary>
        /// <param name="serviceInfo"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static string GetRegisteredType(this IConfiguration configuration) => configuration["RegisteredType"]??"direct";

        /// <summary>
        /// 获取SSOAuthentication是否开启
        /// </summary>
        /// <param name="serviceInfo"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static Boolean IsSSOAuthentication(this IConfiguration configuration) => bool.Parse(configuration["SSOAuthentication"] ?? "false");

        /// <summary>
        /// 获取跨域配置
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static string GetAllowCorsHosts(this IConfiguration configuration) => configuration["CorsHosts"]?? string.Empty;

        /// <summary>
        /// 获取Consul配置
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IConfigurationSection GetConsulSection(this IConfiguration configuration) => configuration.GetSection("Consul");

        /// <summary>
        /// 获取Rabitmq配置
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IConfigurationSection GetRabbitMqSection(this IConfiguration configuration) => configuration.GetSection("RabbitMq");

        /// <summary>
        /// 获取Redis配置
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IConfigurationSection GetRedisSection(this IConfiguration configuration) => configuration.GetSection("Redis");

        /// <summary>
        /// 获取Pgsql配置
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IConfigurationSection GetPgsqlSection(this IConfiguration configuration) => configuration.GetSection("Pgsql");

        /// <summary>
        /// 获取Monogo配置
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IConfigurationSection GetMongoDbSection(this IConfiguration configuration) => configuration.GetSection("MongoDb");

        /// <summary>
        /// 获取JWT配置
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IConfigurationSection GetJWTSection(this IConfiguration configuration) => configuration.GetSection("JWT");

        /// <summary>
        /// 获取线程池配置
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IConfigurationSection GetThreadPoolSettingsSection(this IConfiguration configuration) => configuration.GetSection("ThreadPoolSettings");

        /// <summary>
        /// 获取Hangfire配置
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IConfigurationSection GetHangfireSection(this IConfiguration configuration) => configuration.GetSection("Hangfire");

        /// <summary>
        /// 获取Kestrel配置
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IConfigurationSection GetKestrelSection(this IConfiguration configuration) => configuration.GetSection("Kestrel");
    }
}
