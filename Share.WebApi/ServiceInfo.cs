
using Infra.WebApi;
using System.Reflection;
using Infra.Core.System.Extensions;
using Infra.Core;

namespace Infra.WebApi
{
    /// <summary>
    /// 服务信息
    /// </summary>
    public sealed class ServiceInfo : IServiceInfo
    {
        private static ServiceInfo? _instance = null;
        private static readonly Object _lockObj = new();

        /// <summary>
        ///  服务编号
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// 跨域策略
        /// </summary>
        public string CorsPolicy { get; set; }

        /// <summary>
        /// 简称
        /// </summary>
        public string ShortName { get; private set; }

        /// <summary>
        /// 全称
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; private set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// 程序集名称
        /// </summary>
        public string AssemblyName { get; private set; }
        public string AssemblyFullName { get; private set; }
        public string AssemblyLocation { get; private set; }

        private ServiceInfo()
        {
        }

        static ServiceInfo()
        {
        }

        public static ServiceInfo GetInstance(Assembly startAssembly)
        {
            if (_instance == null)
            {
                lock (_lockObj)
                {
                    if (_instance == null)
                    {
                        if (startAssembly is null)
                            startAssembly = Assembly.GetEntryAssembly();
                        var description = startAssembly.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;
                        var assemblyName = startAssembly.GetName();
                        var version = assemblyName.Version;
                        var fullName = assemblyName.Name.ToLower();
                        var splitFullName = fullName.Split(".");
                        var shortName = splitFullName.Length>3? splitFullName[^2] : splitFullName[^1];

                        _instance = new ServiceInfo
                        {
                            Id = DateTime.Now.GetTotalMilliseconds().ToString(),
                            CorsPolicy = "default",
                            FullName = fullName,
                            ShortName = shortName,
                            AssemblyName = assemblyName.Name,
                            AssemblyFullName = startAssembly.FullName,
                            AssemblyLocation = startAssembly.Location,
                            Description = description,
                            Version = $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision:00}"
                        };
                    }
                }
            }

            return _instance;
        }
    }
}
