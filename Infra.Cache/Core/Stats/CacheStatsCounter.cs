#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2022   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LUJIE-PC
 * 公司名称：云中楼阁
 * 命名空间：Infra.Cache.Core.Stats
 * 唯一标识：dbb7a5b2-082a-4c3a-bedc-33eee5cc0f31
 * 文件名：CacheStatsCounter
 * 当前用户域：LUJIE-PC
 * 
 * 创建者：lujie
 * 电子邮箱：lujie@louge.cloud
 * 创建时间：2022/6/27 20:29:56
 * 版本：V1.0.0
 * 描述：Cache stats counter.
 *
 * ----------------------------------------------------------------
 * 修改人：
 * 时间：
 * 修改说明：
 *
 * 版本：V1.0.1
 *----------------------------------------------------------------*/
#endregion << 版 本 注 释 >>


using System;
using System.Threading;

namespace Infra.Cache.Core
{
    /// <summary>
    /// Cache stats counter.
    /// </summary>
    public class CacheStatsCounter
    {
        /// <summary>
        /// The counters.
        /// </summary>
        private Int64[] _counters = new Int64[2];

        /// <summary>
        /// Increment the specified statsType.
        /// </summary>
        /// <param name="statsType">Stats type.</param>
        public void Increment(StatsType statsType)
        {
            Interlocked.Increment(ref _counters[(Int32)statsType]);
        }

        /// <summary>
        /// Get the specified statsType.
        /// </summary>
        /// <returns>The get.</returns>
        /// <param name="statsType">Stats type.</param>
        public Int64 Get(StatsType statsType)
        {
            return Interlocked.Read(ref _counters[(Int32)statsType]);
        }
    }
}
