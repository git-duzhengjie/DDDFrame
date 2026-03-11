using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Infra.Core.Extensions
{
    public static class ObjectExtension
    {
        public static T DeepCopy<T>(this T obj)
        {
            if (obj == null) return default;
            // 配置忽略循环引用（避免嵌套对象循环引用报错）
            var options = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.IgnoreCycles };
            string json = JsonSerializer.Serialize(obj, options);
            return JsonSerializer.Deserialize<T>(json, options);
        }
    }
}
