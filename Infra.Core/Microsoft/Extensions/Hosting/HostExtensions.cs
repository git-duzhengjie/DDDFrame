
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// 主机扩展
    /// </summary>
    public static class HostExtensions
    {
        /// <summary>
        /// 更新线程池的配置
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static IHost ChangeThreadPoolSettings(this IHost host)
        {
            var poolOptions = host.Services.GetService(typeof(IOptionsMonitor<ThreadPoolSettings>)) as IOptionsMonitor<ThreadPoolSettings>;
            if (poolOptions != null)
                ChangeThreadPoolSettings(host, poolOptions);
            return host;
        }

        /// <summary>
        /// 更新线程池的配置
        /// </summary>
        /// <param name="host"></param>
        /// <param name="poolOptions"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public static IHost ChangeThreadPoolSettings(this IHost host, IOptionsMonitor<ThreadPoolSettings> poolOptions)
        {
            ILogger<IHost> logger = host.Services.GetService(typeof(ILogger<IHost>)) as ILogger<IHost>;
            if (logger == null)
                throw new NullReferenceException(nameof(logger));

            //ThreadPoolSettings poolSetting;
#if NET6_0
            poolOptions.OnChange(poolSetting =>
            {
                ThreadPool.GetMinThreads(out int workerThreads, out int completionPortThreads);
                ThreadPool.GetMaxThreads(out int maxWorkerThreads, out int maxCompletionPortThreads);
                logger.LogInformation("before MinThreads={0},MinCompletionPortThreads={1}", workerThreads, completionPortThreads);
                logger.LogInformation("before MaxThreads={0},MaxCompletionPortThreads={1}", maxWorkerThreads, maxCompletionPortThreads);

                ThreadPool.SetMinThreads(poolSetting.MinThreads, poolSetting.MinCompletionPortThreads);
                ThreadPool.SetMaxThreads(poolSetting.MaxThreads, poolSetting.MaxCompletionPortThreads);

                ThreadPool.GetMinThreads(out int changedWorkerThreads, out int changedCompletionPortThreads);
                ThreadPool.GetMaxThreads(out int changedMaxWorkerThreads, out int changedsMaxCompletionPortThreads);
                logger.LogInformation("changed MinThreads={0},MinCompletionPortThreads={1}", changedWorkerThreads, changedCompletionPortThreads);
                logger.LogInformation("changed MaxThreads={0},MaxCompletionPortThreads={1}", changedMaxWorkerThreads, changedsMaxCompletionPortThreads);
            });

            var poolSetting = poolOptions.CurrentValue;
            ThreadPool.SetMinThreads(poolSetting.MinThreads, poolSetting.MinCompletionPortThreads);
            ThreadPool.SetMaxThreads(poolSetting.MaxThreads, poolSetting.MaxCompletionPortThreads);

            ThreadPool.GetMinThreads(out int workerThreads, out int completionPortThreads);
            ThreadPool.GetMaxThreads(out int maxWorkerThreads, out int maxCompletionPortThreads);

            logger.LogInformation("Setting MinThreads={0},MinCompletionPortThreads={1}", workerThreads, completionPortThreads);
            logger.LogInformation("Setting MaxThreads={0},MaxCompletionPortThreads={1}", maxWorkerThreads, maxCompletionPortThreads);
#endif
            return host;
        }
    }
}
