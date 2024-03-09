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

        public T Deserialize<T>(Byte[] bytes)
        {
            return JsonSerializer.Deserialize<T>(bytes);
        }

        public Object Deserialize(Byte[] bytes, Type type)
        {
            return JsonSerializer.Deserialize<Type>(bytes);
        }

        public Object DeserializeObject(ArraySegment<Byte> value)
        {
            return JsonSerializer.Deserialize<Object>(value);
        }

        public Byte[] Serialize<T>(T value)
        {
            return JsonSerializer.SerializeToUtf8Bytes(value);
        }

        public ArraySegment<Byte> SerializeObject(Object obj)
        {
            return JsonSerializer.SerializeToUtf8Bytes(obj);
        }
    }
}
