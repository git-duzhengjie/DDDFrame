#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2022   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LUJIE-PC
 * 公司名称：云中楼阁
 * 命名空间：Infra.Cache.Core.Stats
 * 唯一标识：c6b7836c-19f4-45fc-8ebf-73d3e8e17c71
 * 文件名：CacheStats
 * 当前用户域：LUJIE-PC
 * 
 * 创建者：lujie
 * 电子邮箱：lujie@louge.cloud
 * 创建时间：2022/6/27 20:29:07
 * 版本：V1.0.0
 * 描述：Cache stats.
 *
 * ----------------------------------------------------------------
 * 修改人：
 * 时间：
 * 修改说明：
 *
 * 版本：V1.0.1
 *----------------------------------------------------------------*/
#endregion << 版 本 注 释 >>

using System.Collections.Concurrent;

namespace Infra.Cache.Core
{
    /// <summary>
    /// Cache stats.
    /// </summary>
    public class CacheStats
    {
        /// <summary>
        /// The counters.
        /// </summary>
        private readonly ConcurrentDictionary<string, CacheStatsCounter> _counters;

        /// <summary>
        /// The default key.
        /// </summary>
        private const string DEFAULT_KEY = "easycahing_catche_stats";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Infra.Cache.Core.CacheStats"/> class.
        /// </summary>
        public CacheStats()
        {
            _counters = new ConcurrentDictionary<string, CacheStatsCounter>();
        }

        /// <summary>
        /// Ons the hit.
        /// </summary>
        public void OnHit()
        {
            GetCounter().Increment(StatsType.Hit);
        }

        /// <summary>
        /// Ons the miss.
        /// </summary>
        public void OnMiss()
        {
            GetCounter().Increment(StatsType.Missed);
        }

        /// <summary>
        /// Gets the statistic.
        /// </summary>
        /// <returns>The statistic.</returns>
        /// <param name="statsType">Stats type.</param>
        public long GetStatistic(StatsType statsType)
        {
            return GetCounter().Get(statsType);
        }

        /// <summary>
        /// Gets the counter.
        /// </summary>
        /// <returns>The counter.</returns>
        private CacheStatsCounter GetCounter()
        {
            if (!_counters.TryGetValue(DEFAULT_KEY, out CacheStatsCounter counter))
            {
                counter = new CacheStatsCounter();
                if (_counters.TryAdd(DEFAULT_KEY, counter))
                {
                    return counter;
                }

                return GetCounter();
            }

            return counter;
        }
    }
}
