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
        //   object:
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

        public static string[] GetEnumAttributeNameList(this Type type)
        {
            var result = new List<string>();
            if (type.IsEnum)
            {
                var fields = type.GetFields();
                foreach (var field in fields)
                {
                    var customAttribute = field.GetCustomAttribute<NameAttribute>(inherit: true);
                    if (customAttribute != null)
                    {
                        result.Add(customAttribute.Name);
                    }
                }
            }
            return [.. result];
        }

        public static (object,string,string)[] GetEnumTextValueList(this Type type)
        {
            var result = new List<(object,string,string)>();
            if (type.IsEnum)
            {
                var fields = type.GetFields();
                foreach (var field in fields)
                {
                    var customAttribute = field.GetCustomAttribute(typeof(NameAttribute), inherit: true) as NameAttribute;
                    if (customAttribute != null)
                    {
                        result.Add((Enum.Parse(type,field.Name),field.Name, customAttribute.Name));
                    }
                }
            }
            return [.. result];
        }

        public static T? GetEnumValue<T>(this string name)
        {
            var type=typeof(T);
            if (type.IsEnum)
            {
                var fields = type.GetFields();
                foreach (var field in fields)
                {
                    var customAttribute = field.GetCustomAttribute(typeof(NameAttribute), inherit: true) as NameAttribute;
                    if (customAttribute != null)
                    {
                        if (customAttribute.Name == name)
                        {
                            return (T)Enum.Parse(type,field.Name);
                        }
                    }
                }
            }
            return default;
        }

        public static bool TryGetEnumValue<T>(this string name,out T value)
        {
            var type = typeof(T);
            if (type.IsEnum)
            {
                var fields = type.GetFields();
                foreach (var field in fields)
                {
                    var customAttribute = field.GetCustomAttribute(typeof(NameAttribute), inherit: true) as NameAttribute;
                    if (customAttribute != null)
                    {
                        if (customAttribute.Name == name)
                        {
                            value= (T)Enum.Parse(type, field.Name);
                            return true;
                        }
                    }
                }
            }
            value = default;
            return false;
        }
    }
}
