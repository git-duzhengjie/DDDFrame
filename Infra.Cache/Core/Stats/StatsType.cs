#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2022   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LUJIE-PC
 * 公司名称：云中楼阁
 * 命名空间：Infra.Cache.Core.Stats
 * 唯一标识：98ddce48-b274-4557-87e5-608294873583
 * 文件名：StatsType
 * 当前用户域：LUJIE-PC
 * 
 * 创建者：lujie
 * 电子邮箱：lujie@louge.cloud
 * 创建时间：2022/6/27 20:30:32
 * 版本：V1.0.0
 * 描述：Stats type.
 *
 * ----------------------------------------------------------------
 * 修改人：
 * 时间：
 * 修改说明：
 *
 * 版本：V1.0.1
 *----------------------------------------------------------------*/
#endregion << 版 本 注 释 >>


namespace Infra.Cache.Core
{
    /// <summary>
    /// Stats type.
    /// </summary>
    public enum StatsType
    {
        /// <summary>
        /// The hit.
        /// </summary>
        Hit = 0,

        /// <summary>
        /// The missed.
        /// </summary>
        Missed = 1,
    }
}
