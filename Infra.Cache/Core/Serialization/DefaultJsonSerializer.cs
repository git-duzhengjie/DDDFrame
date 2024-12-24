#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2022   保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：LUJIE-PC
 * 公司名称：
 * 命名空间：Infra.Cache.Core.Serialization
 * 唯一标识：70b4e5a8-b616-4f6c-9722-c8d3cab0c8bb
 * 文件名：DefaultJsonSerializer
 * 当前用户域：LUJIE-PC
 * 
 * 创建者：lujie
 * 电子邮箱：lujie@louge.cloud
 * 创建时间：2022/6/29 10:02:28
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
using System.Text.Json;
#if NET6_0
using System.Text.Json;
#elif NETSTANDARD2_0
using System.Text;
using Newtonsoft.Json;
#endif 

namespace Infra.Cache.Core.Serialization
{
    public class DefaultJsonSerializer : ICachingSerializer
    {
        public String Name => CachingConstValue.DefaultJsonSerializerName;

        public T Deserialize<T>(byte[] bytes)
        {
            return JsonSerializer.Deserialize<T>(bytes);
        }

        public object Deserialize(byte[] bytes, Type type)
        {
            return JsonSerializer.Deserialize<Type>(bytes);
        }

        public object Deserializeobject(ArraySegment<byte> value)
        {
            return JsonSerializer.Deserialize<object>(value);
        }

        public byte[] Serialize<T>(T value)
        {
            return JsonSerializer.SerializeToUtf8bytes(value);
        }

        public ArraySegment<byte> Serializeobject(object object)
        {
            return JsonSerializer.SerializeToUtf8bytes(object);
        }
    }
}
