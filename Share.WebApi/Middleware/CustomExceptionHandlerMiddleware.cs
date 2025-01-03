﻿
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infra.WebApi.Middleware
{
    /// <summary>
    /// 自定义异常中间件
    /// </summary>
    public class CustomExceptionHandlerMiddleware
    {
        private readonly ILogger<CustomExceptionHandlerMiddleware> logger;
        private readonly IWebHostEnvironment env;
        private readonly RequestDelegate next;

        public CustomExceptionHandlerMiddleware(RequestDelegate next
            , IWebHostEnvironment env
            , ILogger<CustomExceptionHandlerMiddleware> logger)
        {
            this.next = next;
            this.env = env;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var eventId = new EventId(exception.HResult);
            logger.LogError(exception.ToString());
            var status = 500;
            var type = string.Concat("https://httpstatuses.com/", status);
            var title = env.IsDevelopment() ? exception.Message : $"系统异常";
            var detial = env.IsDevelopment() ? exception.GetExceptionDetail() : $"系统异常,请联系管理员({eventId})";

            var problemDetails = new ProblemDetails
            {
                Title = title
                ,
                Detail = detial
                ,
                Type = type
                ,
                Status = status
            };

            context.Response.StatusCode = status;
            context.Response.ContentType = "application/problem+json";
            var errorText = Newtonsoft.Json.JsonConvert.SerializeObject(problemDetails);
            await context.Response.WriteAsync(errorText);
        }
    }

    /// <summary>
    /// 自定义异常扩展
    /// </summary>
    public static class CustomExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
        }
    }
}
