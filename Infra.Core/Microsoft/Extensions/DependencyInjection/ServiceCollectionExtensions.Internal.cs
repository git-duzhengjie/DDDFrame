
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 服务集合扩展 - 受保护的方法
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        private static readonly ConcurrentDictionary<string, char> s_RegisteredModels = new ConcurrentDictionary<string, char>();

        public static bool HasRegistered(this IServiceCollection _, string modelName) => !s_RegisteredModels.TryAdd(modelName.ToLower(), '1');

        internal static T GetSingletonInstanceOrNull<T>(this IServiceCollection services)
            where T : class
        {
            var instance = services.FirstOrDefault(d => d.ServiceType == typeof(T))?.ImplementationInstance;
            if (instance is null)
                return null;

            return (T)instance;
        }

        internal static T GetSingletonInstance<T>(this IServiceCollection services)
            where T : class
        {
            var instance = GetSingletonInstanceOrNull<T>(services);
            if (instance is null)
                throw new InvalidOperationException("Could not find singleton service: " + typeof(T).AssemblyQualifiedName);
            return instance;
        }
    }
}
