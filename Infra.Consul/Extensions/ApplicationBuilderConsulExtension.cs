﻿
using Consul;
using Infra.Consul.Configuration;
using Infra.Consul.Registrar;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// ApplicationBuilder对Consul的扩展
    /// </summary>
    public static class ApplicationBuilderConsulExtension
    {
        /// <summary>
        /// 注册到Consul
        /// </summary>
        /// <param name="app"></param>
        /// <exception cref="NotImplementedException"></exception>
        public static async void RegisterToConsul(this IApplicationBuilder app)
        {
            //var kestrelConfig = app.ApplicationServices.GetRequiredService<IOptions<KestrelConfig>>()?.Value;
            //if (kestrelConfig is null)
            //    throw new NotImplementedException(nameof(kestrelConfig));

            //var registration = app.ApplicationServices.GetRequiredService<ConsulRegistration>();
            //var ipAddresses = registration.GetLocalIpAddress("InterNetwork");
            //if (ipAddresses.IsNullOrEmpty())
            //    throw new NotImplementedException(nameof(kestrelConfig));

            //var defaultEnpoint = kestrelConfig.Endpoints.FirstOrDefault(x => x.Key.EqualsIgnoreCase("default")).Value;
            //if (defaultEnpoint is null || defaultEnpoint.Url.IsNullOrWhiteSpace())
            //    throw new NotImplementedException(nameof(kestrelConfig));

            //var serviceAddress = new Uri(defaultEnpoint.Url);
            //if (serviceAddress.Host == "0.0.0.0")
            //    serviceAddress = new Uri($"{serviceAddress.Scheme}://{ipAddresses.FirstOrDefault()}:{serviceAddress.Port}");

            //registration.Register(serviceAddress);
            await app.ApplicationServices.GetRequiredService<IConsulRegister>()!.RegisterAsync();
        }

        /// <summary>
        /// 注册到Consul
        /// </summary>
        /// <param name="app"></param>
        /// <param name="serviceAddress"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void RegisterToConsul(this IApplicationBuilder app, Uri serviceAddress)
        {
            if (serviceAddress is null)
                throw new ArgumentNullException(nameof(serviceAddress));

            var registration = app.ApplicationServices.GetRequiredService<ConsulRegistration>();
            registration.Register(serviceAddress);
        }

        /// <summary>
        /// 注册到Consul
        /// </summary>
        /// <param name="app"></param>
        /// <param name="instance"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void RegisterToConsul(this IApplicationBuilder app, AgentServiceRegistration instance)
        {
            if (instance is null)
                throw new ArgumentNullException(nameof(instance));

            var registration = app.ApplicationServices.GetRequiredService<ConsulRegistration>();
            registration.Register(instance);
        }

        public static void UseHealthCheck(this IApplicationBuilder app, string checkPath = "/healthcheck")
        {
            app.Map(checkPath, applicationBuilder => applicationBuilder.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                await context.Response.WriteAsync("OK");
            }));
        }

    }
}
