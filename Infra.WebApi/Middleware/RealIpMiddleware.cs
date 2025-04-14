
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Infra.WebApi.WebApi.Middleware
{
    /// <summary>
    /// 真实IP中间件
    /// </summary>
    public class RealIpMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RealIpMiddleware> _logger;
        private readonly FilterOption _option;

        public RealIpMiddleware(RequestDelegate next, ILogger<RealIpMiddleware> logger, FilterOption option)
        {
            _next = next;
            _logger = logger;
            _option = option;
        }

        public async Task Invoke(HttpContext context)
        {
            var headers = context.Request.Headers;
            _logger.LogDebug($"请求Host:{context.Request.Host.Host}");
            _logger.LogDebug($"请求Token:{context.Request.Headers["Authorization"]}");
            if (_option.HeaderKeys != null && _option.HeaderKeys.Length > 0)
            {
                foreach (var headerKey in _option.HeaderKeys)
                {
                    var ips = headers[headerKey].FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(ips))
                    {
                        var realIp = ips.Split(",", StringSplitOptions.RemoveEmptyEntries)[0];
                        context.Connection.RemoteIpAddress = IPAddress.Parse(realIp);
                        _logger.LogDebug($"Resolve real ip success: {context.Connection.RemoteIpAddress}");
                        break;
                    }
                }
            }
            await _next(context);
        }
    }

    public class FilterOption
    {
        public string[] HeaderKeys { get; set; }
    }

    /// <summary>
    /// 真实IP中间件扩展
    /// </summary>
    public static class RealIpMiddlewareExtensions
    {
        public static IApplicationBuilder UseRealIp(this IApplicationBuilder builder, Action<FilterOption> configureOption = null)
        {
            var option = new FilterOption();
            configureOption?.Invoke(option);
            return builder.UseMiddleware<RealIpMiddleware>(option);
        }
    }
}
