#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2022   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LUJIE-PC
 * 公司名称：云中楼阁
 * 命名空间：Infra.Cache.Core.Serialization
 * 唯一标识：4bba408e-d3b6-47e4-ad2f-da5142d3a316
 * 文件名：ICachingSerializer
 * 当前用户域：LUJIE-PC
 * 
 * 创建者：lujie
 * 电子邮箱：lujie@louge.cloud
 * 创建时间：2022/6/16 16:01:27
 * 版本：V1.0.0
 * 描述：缓存序列化
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

namespace Infra.Cache.Core.Serialization
{
    /// <summary>
    /// 缓存序列化
    /// </summary>
    public interface ICachingSerializer
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        String Name { get; }

        /// <summary>
        /// Serialize the specified value.
        /// </summary>
        /// <returns>The serialize.</returns>
        /// <param name="value">Value.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        byte[] Serialize<T>(T value);

        /// <summary>
        /// Deserialize the specified bytes.
        /// </summary>
        /// <returns>The deserialize.</returns>
        /// <param name="bytes">bytes.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        T Deserialize<T>(byte[] bytes);

        /// <summary>
        /// Deserialize the specified bytes.
        /// </summary>
        /// <returns>The deserialize.</returns>
        /// <param name="bytes">bytes.</param>
        /// <param name="type">Type.</param>
        object Deserialize(byte[] bytes, Type type);

        /// <summary>
        /// Serializes the object.
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="object">object.</param>
        ArraySegment<byte> SerializeObject(object obj);

        /// <summary>
        /// Deserializes the object.
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="value">Value.</param>
        object DeserializeObject(ArraySegment<byte> value);
    }
}
