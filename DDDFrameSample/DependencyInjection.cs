﻿using Infra.WebApi.DependInjection;

namespace DDDFrameSample
{
    /// <summary>
    /// 依赖注入实现
    /// </summary>
    public class DependencyInjection : DependencyInjectionBase
    {
        public DependencyInjection(IServiceCollection services) : base(services)
        {
        }
    }
}
