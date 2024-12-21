using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;

namespace Infra.Core.Json
{
    internal sealed class FrameJsonMvcOptionsSetup : IConfigureOptions<MvcOptions>
    {
        public FrameJsonMvcOptionsSetup() { 
        }
        public void Configure(MvcOptions options)
        {
            options.OutputFormatters.RemoveType<SystemTextJsonOutputFormatter>();
            options.OutputFormatters.Add(new FrameJson());
            options.InputFormatters.RemoveType<SystemTextJsonInputFormatter>();
            options.InputFormatters.Add(new FrameJson());
        }
    }
}
