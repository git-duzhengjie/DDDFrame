using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System
{
    /// <summary>
    /// 反射扩展类
    /// </summary>
    public static class ReflectionExtension
    {
        public static Boolean IsNotAbstractClass(this Type type, Boolean publicOnly)
        {
            if (type.IsSpecialName)
                return false;

            if (type.IsClass && !type.IsAbstract)
            {
                if (type.HasAttribute<CompilerGeneratedAttribute>())
                    return false;

                if (publicOnly)
                    return type.IsPublic || type.IsNestedPublic;

                return true;
            }
            return false;
        }

        public static Boolean HasAttribute(this Type type, Type attributeType) => type.IsDefined(attributeType, inherit: true);

        public static Boolean HasAttribute<T>(this Type type) where T : Attribute => type.HasAttribute(typeof(T));

        public static Boolean HasAttribute<T>(this Type type, Func<T, Boolean> predicate) where T : Attribute => type.GetCustomAttributes<T>(inherit: true).Any(predicate);
    }
}
