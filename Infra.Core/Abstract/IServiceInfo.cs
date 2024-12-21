
using System;

namespace Infra.Core.Abstract
{
    /// <summary>
    /// 服务接口
    /// </summary>
    public interface IServiceInfo
    {
        /// <summary>
        /// 服务编号
        /// </summary>
        string Id { get; }

        /// <summary>
        /// 跨域策略
        /// </summary>
        string CorsPolicy { get; set; }

        /// <summary>
        /// 简称
        /// </summary>
        string ShortName { get; }

        /// <summary>
        /// 全程
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// 版本
        /// </summary>
        string Version { get; }

        /// <summary>
        /// 描述
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 程序集名称
        /// </summary>
        string AssemblyName { get; }

        /// <summary>
        /// 程序集全称
        /// </summary>
        string AssemblyFullName { get; }

        /// <summary>
        /// 程序集路径
        /// </summary>
        string AssemblyLocation { get; }
    }
}
