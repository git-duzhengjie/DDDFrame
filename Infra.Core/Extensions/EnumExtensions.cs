using Infra.Core.Attributes;
using System.Reflection;

namespace Infra.Core.Extensions
{
    public static class EnumExtensions
    {
        //
        // 摘要:
        //     获取字段或属性名称
        //
        // 参数:
        //   obj:
        //     欲获取对象
        public static int ToInt(this object obj)
        {
            if (obj != null && obj.GetType().IsEnum)
            {
                return (int)obj;
            }

            return -1;
        }
        //
        // 摘要:
        //     获取字段或属性名称
        //
        // 参数:
        //   obj:
        //     欲获取对象
        public static string ToName(this object obj)
        {
            if (obj != null && obj.GetType().IsEnum)
            {
                FieldInfo field = obj.GetType().GetField(obj.ToString());
                if (field != null)
                {
                    object[] customAttributes = field.GetCustomAttributes(typeof(NameAttribute), inherit: true);
                    if (customAttributes != null && customAttributes.Length != 0)
                    {
                        return (customAttributes[0] as NameAttribute)?.Name ?? string.Empty;
                    }
                }
            }

            return string.Empty;
        }

        public static string[] GetEnumNameList(this Type type)
        {
            var result=new List<string>();
            if (type.IsEnum)
            {
                var fields=type.GetFields();
                foreach (var field in fields)
                {
                    object[] customAttributes = field.GetCustomAttributes(typeof(NameAttribute), inherit: true);
                    if (customAttributes != null && customAttributes.Length != 0)
                    {
                        result.Add(field.Name);
                    }
                }
            }
            return [.. result];
        }

    }
}
