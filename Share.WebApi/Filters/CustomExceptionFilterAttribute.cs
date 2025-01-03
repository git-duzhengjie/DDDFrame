using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Mvc.Filters
{
    /// <summary>
    /// 异常拦截器 拦截 StatusCode>=500 的异常
    /// </summary>
    public sealed class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<CustomExceptionFilterAttribute> _logger;
        private readonly IWebHostEnvironment _env;

        public CustomExceptionFilterAttribute(ILogger<CustomExceptionFilterAttribute> logger
            , IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public override void OnException(ExceptionContext context)
        {
            var status = 500;
            var exception = context.Exception;
            var eventId = new EventId(exception.HResult);
            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
            var hostAndPort = context.HttpContext.Request.Host.HasValue ? context.HttpContext.Request.Host.Value : string.Empty;
            var requestUrl = string.Concat(hostAndPort, context.HttpContext.Request.Path);
            var type = string.Concat("https://httpstatuses.com/", status);

            string title;
            string detial;
            _logger.LogError(exception.ToString());
            title = _env.IsDevelopment() ? exception.Message : $"系统异常";
            detial = _env.IsDevelopment() ? exception.GetExceptionDetail() : $"系统异常,请联系管理员({eventId})";

            var problemDetails = new ProblemDetails
            {
                Title = title
                ,
                Detail = detial
                ,
                Type = type
                ,
                Status = status
                ,
                Instance = requestUrl
            };

            context.Result = new ObjectResult(problemDetails) { StatusCode = status };
            context.ExceptionHandled = true;
        }

        public override Task OnExceptionAsync(ExceptionContext context)
        {
            OnException(context);
            return Task.CompletedTask;
        }
    }
}
