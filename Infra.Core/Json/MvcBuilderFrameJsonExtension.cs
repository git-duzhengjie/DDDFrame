using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Core.Json
{
    public static class MvcBuilderFrameJsonExtension
    {
        public static IMvcBuilder AddFrameJson(this IMvcBuilder builder,Action<MvcOptions> action)
        {
            builder.Services.Configure(action);
            return builder;
        }
    }
}
