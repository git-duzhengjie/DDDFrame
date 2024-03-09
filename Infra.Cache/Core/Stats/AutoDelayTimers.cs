#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2022   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LUJIE-PC
 * 公司名称：
 * 命名空间：Infra.Cache.Core.Stats
 * 唯一标识：faaa91da-21fd-4a9c-98b4-5ee78c8bd7a0
 * 文件名：AutoDelayTimers
 * 当前用户域：LUJIE-PC
 * 
 * 创建者：lujie
 * 电子邮箱：lujie@louge.cloud
 * 创建时间：2022/6/28 9:08:01
 * 版本：V1.0.0
 * 描述：
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
using System.Collections.Concurrent;
using System.Threading;

namespace Infra.Cache.Core
{
    public class AutoDelayTimers
    {
        private static readonly Lazy<AutoDelayTimers> lazy = new Lazy<AutoDelayTimers>(() => new AutoDelayTimers());
        private static ConcurrentDictionary<String, Timer> _timers;

        static AutoDelayTimers()
        {
        }

        private AutoDelayTimers()
        {
            _timers = new ConcurrentDictionary<String, Timer>();
        }

        public static AutoDelayTimers Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public bool TryAdd(String key, Timer dealytimer)
        {
            return _timers.TryAdd(key, dealytimer);
        }

        public void CloseTimer(String key)
        {
            if (_timers.ContainsKey(key))
            {
                if (_timers.TryRemove(key, out Timer timer))
                {
                    timer?.Dispose();
                }
            }
        }

        public bool ContainsKey(String key)
        {
            return _timers.ContainsKey(key);
        }
    }
}
