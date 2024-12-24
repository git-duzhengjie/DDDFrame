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
        //   object:
        //     欲获取对象
        public static int ToInt(this object object)
        {
            if (object != null && object.GetType().IsEnum)
            {
                return (int)object;
            }

            return -1;
        }
        //
        // 摘要:
        //     获取字段或属性名称
        //
        // 参数:
        //   object:
        //     欲获取对象
        public static string ToName(this object object)
        {
            if (object != null && object.GetType().IsEnum)
            {
                FieldInfo field = object.GetType().GetField(object.ToString());
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
