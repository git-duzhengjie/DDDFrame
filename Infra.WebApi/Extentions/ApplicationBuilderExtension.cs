using Infra.WebApi.WebApi.Middleware;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Infra.WebApi.DependencyInjection;
using Infra.WebApi.Middleware;
using Infra.Consul.Configuration;
using Infra.Core.Extensions;
using Prometheus;
using Prometheus.DotNetRuntime;
using Infra.WebApi.Consts.RegistrationCenter;
using Infra.Core.Abstract;
using UniversalRPC.Extensions;
using UniversalRPC.Services;
using Microsoft.Extensions.FileProviders;
using Infra.EF.Context;
using Microsoft.EntityFrameworkCore;
using Infra.Core.Json;
using UniversalRPC.Serialization;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// 应用创建扩展
    /// </summary>
    public static class ApplicationBuilderExtension
    {

        /// <summary>
        /// 统一注册通用中间件
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configuration"></param>
        /// <param name="serviceInfo"></param>
        /// <param name="endpointRoute"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseDefault(this IApplicationBuilder app
            , Action<IApplicationBuilder> beforeAuthentication = null
            , Action<IApplicationBuilder> afterAuthorization = null
            , Action<IEndpointRouteBuilder> endpointRoute = null,
            bool useURPCServiceName=true)
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
            app.UseRouting();
            app.UseCors(serviceInfo.CorsPolicy);
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
                UseURPCSerivice(endpoints,app.ApplicationServices,useURPCServiceName);
            });
            app.UseRegistrationCenter();
            app.UseHttpsRedirection();
            app.UseHealthCheck(healthUrl);
            app.UseMigration();
            return app;
        }

        public static void UseMigration(this IApplicationBuilder app)
        {
            
            using var scope=app.ApplicationServices.CreateScope();
            var serviceInfo= scope.ServiceProvider.GetService<IServiceInfo>();
            var migrationAssembly = serviceInfo.GetMigrationAssembly();
            if (migrationAssembly != null)
            {
                var dbContext = scope.ServiceProvider.GetService<FrameDbContextBase>();
                if (dbContext.Transaction)
                {
                    dbContext?.Database.Migrate();
                }
            }
        }
        private static void UseURPCSerivice(IEndpointRouteBuilder endpoints, IServiceProvider applicationServices, bool useURPCServiceName,ISerialize serialize=null)
        {
            var serviceFactory = applicationServices.GetService<URPCServiceFactory>();
            var types = serviceFactory?.GetURPCServiceITypes();
            if (types != null && types.Length > 0)
            {
                if (useURPCServiceName)
                {
                    var type = types[0];
                    var serviceName = type.GetServiceName();
                    endpoints.UseURPCService(serviceName,serialize??new FrameJson());
                }
                else
                {
                    endpoints.UseURPCService("",serialize??new FrameJson());
                }
            }
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
                    break;
                default:
                    break;
            }
            return app;
        }

       
    }
}
