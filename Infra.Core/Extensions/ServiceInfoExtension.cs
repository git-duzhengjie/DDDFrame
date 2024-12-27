using Infra.Core.Abstract;
using System.Reflection;

namespace Infra.Core.Extensions
{
    /// <summary>
    /// 服务扩展
    /// </summary>
    public static class ServiceInfoExtension
    {
        private static object lockobject = new();
        private static Assembly webApiAssembly;
        private static Assembly appAssembly;
        private static Assembly domainAssembly;
        private static Assembly migrationAssembly;

        /// <summary>
        /// 获取WebApiAssembly程序集
        /// </summary>
        /// <returns></returns>
        public static Assembly GetWebApiAssembly(this IServiceInfo _)
        {
            if (webApiAssembly is null)
            {
                lock (lockobject)
                {
                    if (webApiAssembly is null)
                        webApiAssembly = Assembly.GetEntryAssembly();
                }
            }
            return webApiAssembly;
        }

        /// <summary>
        /// 获取Application程序集
        /// </summary>
        /// <returns></returns>
        public static Assembly GetApplicationAssembly(this IServiceInfo serviceInfo)
        {
            if (appAssembly is null)
            {
                lock (lockobject)
                {
                    if (appAssembly is null)
                    {
                        var appAssemblyName = serviceInfo.AssemblyName + ".Application";
                        var appAssemblyFullName = serviceInfo.AssemblyFullName.Replace(serviceInfo.AssemblyName,appAssemblyName);
                        appAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName.EqualsIgnoreCase(appAssemblyFullName));
                        if (appAssembly is null)
                            appAssembly = Assembly.Load(appAssemblyName);
                    }
                }
            }
            return appAssembly;
        }
        /// <summary>
        /// 获取Domain程序集
        /// </summary>
        /// <returns></returns>
        public static Assembly GetDomainAssembly(this IServiceInfo serviceInfo)
        {
            if (domainAssembly is null)
            {
                lock (lockobject)
                {
                    if (domainAssembly is null)
                    {
                        var domainAssemblyName = serviceInfo.AssemblyName + ".Domain";
                        var domainAssemblyFullName = serviceInfo.AssemblyFullName.Replace(serviceInfo.AssemblyName, domainAssemblyName);
                        domainAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName.EqualsIgnoreCase(domainAssemblyFullName));
                    }
                }
            }
            return domainAssembly;
        }

        /// <summary>
        /// 获取Migration程序集
        /// </summary>
        /// <returns></returns>
        public static Assembly GetMigrationAssembly(this IServiceInfo serviceInfo)
        {
            if (migrationAssembly is null)
            {
                lock (lockobject)
                {
                    if (migrationAssembly is null)
                    {
                        var domainAssemblyName = serviceInfo.AssemblyName + ".Migration";
                        var domainAssemblyFullName = serviceInfo.AssemblyFullName.Replace(serviceInfo.AssemblyName, domainAssemblyName);
                        migrationAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName.EqualsIgnoreCase(domainAssemblyFullName));
                    }
                }
            }
            return migrationAssembly;
        }
    }
}
