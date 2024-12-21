
using Infra.WebApi.WebApi;
using Infra.WebApi.WebApi.Middleware;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using Infra.WebApi;
using Infra.WebApi.Configuration;
using Infra.WebApi.DependencyInjection;
using Infra.WebApi.Middleware;
using Infra.Consul.Configuration;
using Infra.WebApi.Extensions;
using Prometheus;
using Prometheus.DotNetRuntime;
using Infra.WebApi.Consts.RegistrationCenter;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Http;
using Infra.Core.Abstract;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// 应用创建扩展
    /// </summary>
    public static class ApplicationBuilderExtension
    {

        /// <summary>
        /// 统一注册Louge.WebApi通用中间件
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configuration"></param>
        /// <param name="serviceInfo"></param>
        /// <param name="endpointRoute"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseDefault(this IApplicationBuilder app
            , Action<IApplicationBuilder> beforeAuthentication = null
            , Action<IApplicationBuilder> afterAuthorization = null
            , Action<IEndpointRouteBuilder> endpointRoute = null)
        {
            ServiceLocator.Provider = app.ApplicationServices;
            var configuration = app.ApplicationServices.GetService<IConfiguration>();
            var environment = app.ApplicationServices.GetService<IHostEnvironment>();
            var serviceInfo = app.ApplicationServices.GetService<IServiceInfo>();
            var healthUrl = configuration.GetConsulSection().Get<ConsulConfig>().HealthCheckUrl;

            if (environment.IsDevelopment())
                IdentityModelEventSource.ShowPII = true;

            var defaultFilesOptions = new DefaultFilesOptions();
            defaultFilesOptions.DefaultFileNames.Clear();
            defaultFilesOptions.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(defaultFilesOptions);
            app.UseStaticFiles();
            app.UseCustomExceptionHandler();
            app.UseRealIp(x => x.HeaderKeys = ["X-Forwarded-For", "X-Real-IP"]);
            app.UseCors(serviceInfo.CorsPolicy);
            app.UseMiniProfiler();
            app.UseSwagger(
                c =>
            {
                c.RouteTemplate = $"/{serviceInfo.ShortName}/swagger/{{documentName}}/swagger.json";
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                {
                    swaggerDoc.Servers = [new() { Url = $"/", Description = serviceInfo.Description }];
                });
            }
            );
            app.UseSwaggerUI(
                c =>
            {
                var assembly = serviceInfo.GetWebApiAssembly();
                c.IndexStream = () => assembly.GetManifestResourceStream($"{assembly.GetName().Name}.swagger_miniprofiler.html");
                c.SwaggerEndpoint($"/{serviceInfo.ShortName}/swagger/{serviceInfo.Version}/swagger.json", $"{serviceInfo.FullName}-{serviceInfo.Version}");
                c.RoutePrefix = $"{serviceInfo.ShortName}";
            }
            );
            //app.UseHealthChecks($"/{healthUrl}", new HealthCheckOptions()
            //{
            //    Predicate = _ => true,
            //    // 该响应输出是一个json，包含所有检查项的详细检查结果
            //    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            //});
            app.UseRouting();
            app.UseHttpMetrics();
            DotNetRuntimeStatsBuilder.Customize()
                                                .WithContentionStats()
                                                .WithGcStats()
                                                .WithThreadPoolStats()
                                                .StartCollecting()
                                                ;
            beforeAuthentication?.Invoke(app);
            app.UseAuthentication();
            app.UseSSOAuthentication(configuration.IsSSOAuthentication());
            app.UseAuthorization();
            afterAuthorization?.Invoke(app);
            app.UseEndpoints(endpoints =>
            {
                endpointRoute?.Invoke(endpoints);
                endpoints.MapMetrics();
                endpoints.MapControllers()
                .RequireAuthorization()
                ;
            });
            app.UseRegistrationCenter();
            app.UseHttpsRedirection();
            app.UseHealthCheck(healthUrl);
            return app;
        }

        /// <summary>
        /// 注册服务到注册中心
        /// </summary>
        public static IApplicationBuilder UseRegistrationCenter(this IApplicationBuilder app)
        {
            var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
            var registeredType = configuration.GetRegisteredType().ToLower();
            switch (registeredType)
            {
                case RegisteredTypeConsts.Consul:
                    app.RegisterToConsul();
                    break;
                case RegisteredTypeConsts.Nacos:
                    // TODO
                    //app.RegisterToNacos();
                    break;
                default:
                    break;
            }
            return app;
        }

       
    }
}
