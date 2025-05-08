#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2022   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LUJIE-PC
 * 公司名称：
 * 命名空间：Infra.Cache.Core.Stats
 * 唯一标识：0521ffb3-2ef9-421d-98b0-3d1869e632e8
 * 文件名：LocalVariables
 * 当前用户域：LUJIE-PC
 * 
 * 创建者：lujie
 * 电子邮箱：lujie@louge.cloud
 * 创建时间：2022/6/27 20:07:51
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
using System.Collections.Generic;
using System.Linq;

namespace Infra.Cache.Core
{
    public sealed class LocalVariables
    {
        private static readonly Lazy<LocalVariables> lazy = new Lazy<LocalVariables>(() => new LocalVariables());
        private static readonly ConcurrentQueue<Model> _queue = new ConcurrentQueue<Model>();

        static LocalVariables()
        {
        }

        private LocalVariables()
        {
        }

        public static LocalVariables Instance => lazy.Value;

        public ConcurrentQueue<Model> Queue => _queue;

        public sealed class Model
        {
            //[2022.0707][Jay:修改][为了支持.netstandard2.0，语法不支持]
            //public List<string> CacheKeys { get; init; }
            //public DateTime ExpireDt { get; init; }

            public List<string> CacheKeys { get; set; }
            public DateTime ExpireDt { get; set; }

            public Model(IEnumerable<string> cacheKeys, DateTime expireDt)
            {
                CacheKeys = cacheKeys.ToList();
                ExpireDt = expireDt;
            }
        }
    }
}
