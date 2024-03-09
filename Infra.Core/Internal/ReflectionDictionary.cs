using System.Collections.Concurrent;
using System.Reflection;

namespace Infra.Core.Internal
{
    internal static class ReflectionDictionary
    {
        internal static readonly ConcurrentDictionary<Type, PropertyInfo[]> TypePropertyCache = new ConcurrentDictionary<Type, PropertyInfo[]>();

        internal static readonly ConcurrentDictionary<Type, FieldInfo[]> TypeFieldCache = new ConcurrentDictionary<Type, FieldInfo[]>();

        internal static readonly ConcurrentDictionary<Type, MethodInfo[]> TypeMethodCache = new ConcurrentDictionary<Type, MethodInfo[]>();

        internal static readonly ConcurrentDictionary<Type, ConstructorInfo> TypeConstructorCache = new ConcurrentDictionary<Type, ConstructorInfo>();

        internal static readonly ConcurrentDictionary<Type, Func<Object>> TypeEmptyConstructorFuncCache = new ConcurrentDictionary<Type, Func<Object>>();

        internal static readonly ConcurrentDictionary<Type, Func<Object[], Object>> TypeConstructorFuncCache = new ConcurrentDictionary<Type, Func<Object[], Object>>();

        internal static readonly ConcurrentDictionary<PropertyInfo, Func<Object, Object>> PropertyValueGetters = new ConcurrentDictionary<PropertyInfo, Func<Object, Object>>();

        internal static readonly ConcurrentDictionary<PropertyInfo, Action<Object, Object>> PropertyValueSetters = new ConcurrentDictionary<PropertyInfo, Action<Object, Object>>();

        internal static readonly ConcurrentDictionary<Type, Object> TypeObejctCache = new ConcurrentDictionary<Type, Object>();
    }

    internal static class StrongTypedDictionary<T>
    {
        public static readonly ConcurrentDictionary<PropertyInfo, Func<T, Object>> PropertyValueGetters = new ConcurrentDictionary<PropertyInfo, Func<T, Object>>();

        public static readonly ConcurrentDictionary<PropertyInfo, Action<T, Object>> PropertyValueSetters = new ConcurrentDictionary<PropertyInfo, Action<T, Object>>();
    }
}
