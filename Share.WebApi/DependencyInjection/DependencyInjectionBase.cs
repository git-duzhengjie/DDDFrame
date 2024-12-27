using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Infra.WebApi.Configuration;
using Infra.WebApi.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using System.Configuration;
using ConfigurationSection = System.Configuration.ConfigurationSection;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using LiteX.HealthChecks.Redis;
using Infra.Core.Extensions;
using Infra.Consul.Configuration;
using Infra.WebApi.Extentions;
using UniversalRPC.Extensions;
using Infra.Core.Abstract;
using Infra.Core.Json;
using System.Text.Json;
using Infra.WebApi.Service;
using Infra.IdGenerater.Extensions;
using Grpc.Net.Client.Configuration;
using ServiceConfig = Infra.WebApi.Configuration.ServiceConfig;
using Infra.EF.PG.Service;
using Infra.EF.PG.Context;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Infra.Core.Attributes;
using UniversalRPC.Services;
using Microsoft.EntityFrameworkCore;
using Consul;
using UniversalRPC;
using Infra.WebApi.Models;

namespace Infra.WebApi.DependInjection
{
    /// <summary>
    /// WebApi依赖注入抽象类
    /// </summary>
    public abstract class DependencyInjectionBase :IDependencyInjection
    {
        public static string Name => "webapi";
        protected IConfiguration Configuration;
        protected readonly IServiceCollection Services;
        protected readonly IHostEnvironment HostEnvironment;
        protected readonly IServiceInfo ServiceInfo;

        /// <summary>
        /// 服务注册与系统配置
        /// </summary>
        /// <param name="configuration"><see cref="IConfiguration"/></param>
        /// <param name="services"><see cref="IServiceInfo"/></param>
        /// <param name="environment"><see cref="IHostEnvironment"/></param>
        /// <param name="serviceInfo"><see cref="ServiceInfo"/></param>
        protected DependencyInjectionBase(IServiceCollection services)
        {
            Services = services;
            Configuration = services.GetConfiguration();
            ServiceInfo = services.GetServiceInfo();
        }

        /// <summary>
        /// 注册Webapi通用的服务
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        public virtual void AddWebApiDefault()
        {
            LoadAllAssemblies();
            Services.AddHttpContextAccessor();
            Services.AddMemoryCache();
            Configure();
            AddDbContext();
            AddRPCClients();
            AddDomainServiceContext();
            AddStrategies();
            AddFactories();
            AddDomainServices();
            AddRPCServices();
            AddApplicationServices();
            AddControllers();
            AddAuthentication();
            AddAuthorization<PermissionHandlerRemote>();
            AddCors();
            AddSwaggerGen();
            //AddHealthChecks();
            AddMiniProfiler();
            AddConsul();
            AddIdGeneraterService();
        }

        private void LoadAllAssemblies()
        {
            var directory=AppDomain.CurrentDomain.BaseDirectory;
            var assemblyFiles = Directory.GetFiles(directory).Where(x=>x.EndsWith(".dll")).ToArray();
            var loadAssemblies=AppDomain.CurrentDomain.GetAssemblies();
            foreach(var assemblyFile in assemblyFiles)
            {
                var name=Path.GetFileNameWithoutExtension(assemblyFile);
                if (!loadAssemblies.Any(l => l.FullName.Contains(name)))
                {
                    Assembly.LoadFrom(assemblyFile);
                }
            }
        }

        private void AddDbContext()
        {
            var config= Configuration.GetPgsqlSection().Get<PgsqlConfig>();
            Services.AddDbContext<FrameDbContext>(option =>
            {
                option.UseNpgsql(config.ConnectionString, b =>
                {
                    b.MigrationsAssembly(GetMigrationAssembName());
                });
            });
            Services.AddScoped<EntityFactory>();
            Services.AddScoped<IDbData, DbData>();
        }

        private string? GetMigrationAssembName()
        {
            return ServiceInfo.AssemblyName + ".Migrations";
        }

        private void AddRPCServices()
        {
            Services.AddURPCService();
        }

        private void AddDomainServiceContext()
        {
            var domainAssembly = ServiceInfo.GetDomainAssembly();
            if (domainAssembly is not null)
            {
                var modelTypes = domainAssembly.GetExportedTypes()
                    .Where(m => m.IsAssignableTo(typeof(IDomainServiceContext)) && m.IsNotAbstractClass(true))
                    .ToArray();
                if (modelTypes.Length > 1)
                {
                    throw new InvalidOperationException("一个服务只能有一个Context");
                }
                foreach (var modelType in modelTypes)
                {
                    Services.AddScoped(typeof(IDomainServiceContext),modelType);
                }
            }
        }

        private void AddIdGeneraterService()
        {
            Services.AddInfraYitterIdGenerater(Configuration.GetRedisSection());
        }

        private void AddDomainServices()
        {
            var domainAssembly = ServiceInfo.GetDomainAssembly();
            if (domainAssembly is not null)
            {
                var modelTypes = domainAssembly.GetTypes()
                    .Where(m => m.IsAssignableTo(typeof(IDomainService)) && m.IsNotAbstractClass(true))
                    .ToArray();
                foreach (var modelType in modelTypes)
                {
                    if (modelType != null)
                    {
                        Services.AddScoped(modelType);
                    }
                }
            }
        }
        private void AddStrategies()
        {
            var appAssembly = ServiceInfo.GetApplicationAssembly();
            if (appAssembly is not null)
            {
                var modelTypes = appAssembly.GetExportedTypes()
                    .Where(m => m.IsAssignableTo(typeof(IStrategy)) && m.IsNotAbstractClass(true))
                    .ToArray();
                var injectTypes = new List<(Type, int)>();
                foreach (var modelType in modelTypes)
                {
                    if (modelType != null)
                    {
                        Services.AddScoped(modelType);
                    }
                }
            }
            var domainAssembly = ServiceInfo.GetDomainAssembly();
            if (domainAssembly is not null)
            {
                var modelTypes = domainAssembly.GetExportedTypes()
                    .Where(m => m.IsAssignableTo(typeof(IStrategy)) && m.IsNotAbstractClass(true))
                    .ToArray();
                var injectTypes = new List<(Type, int)>();
                foreach (var modelType in modelTypes)
                {
                    if (modelType != null)
                    {
                        Services.AddScoped(modelType);
                    }
                }
            }
        }
        private void AddFactories()
        {
            var appAssembly = ServiceInfo.GetApplicationAssembly();
            if (appAssembly is not null)
            {
                var modelTypes = appAssembly.GetExportedTypes()
                    .Where(m => m.IsAssignableTo(typeof(IFactory)) && m.IsNotAbstractClass(true))
                    .ToArray();
                foreach (var modelType in modelTypes)
                {
                    if (modelType != null)
                    {
                        Services.AddScoped(modelType);
                    }
                }
            }
            var domainAssembly = ServiceInfo.GetDomainAssembly();
            if (domainAssembly is not null)
            {
                var modelTypes = domainAssembly.GetExportedTypes()
                    .Where(m => m.IsAssignableTo(typeof(IFactory)) && m.IsNotAbstractClass(true)&&!m.IsAssignableTo(typeof(IDomainServiceFactory)))
                    .ToArray();
                foreach (var modelType in modelTypes)
                {
                    if (modelType != null)
                    {
                        Services.AddScoped(modelType);
                    }
                }
                var domainServiceFactoryType = domainAssembly.GetTypes()
                    .FirstOrDefault(m => m.IsAssignableTo(typeof(IDomainServiceFactory)) && m.IsNotAbstractClass(true));
                if (domainServiceFactoryType != null)
                {
                    Services.AddScoped(typeof(IDomainServiceFactory), domainServiceFactoryType);
                }
            }
        }

        private void AddRPCClients()
        {
            var serviceConfigSection = Configuration.GetServerConfigSection();
            ServiceConfig config = serviceConfigSection.Get<ServiceConfig>();
            Services.AddURPCClients(config.GatewayUrl,new FrameJson());
        }

        private void AddConsul()
        {
            Services.AddInfraConsul();
        }

        /// <summary>
        /// 注册配置类到IOC容器
        /// </summary>
        protected virtual void Configure()
        {
            Services.Configure<JwtConfig>(Configuration.GetJWTSection());
            Services.Configure<ConsulConfig>(Configuration.GetConsulSection());
            Services.Configure<ThreadPoolSettings>(Configuration.GetThreadPoolSettingsSection());
            //Services.Configure<KestrelConfig>(Configuration.GetKestrelSection());
        }

        /// <summary>
        /// Controllers 注册
        /// Sytem.Text.Json 配置
        /// FluentValidation 注册
        /// ApiBehaviorOptions 配置
        /// </summary>
        protected virtual void AddControllers()
        {
            Services.Configure<JsonSerializerOptions>((options) =>
            {
                options.SetJsonSerializerOptions();
            });
            Services.AddControllers(options => options.Filters.Add(typeof(CustomExceptionFilterAttribute)))
                .AddFrameJson(options =>
                {
                    new FrameJsonMvcOptionsSetup().Configure(options);
                });

            //参数验证返回信息格式调整
            Services.Configure<ApiBehaviorOptions>(options =>
            {
                //关闭自动验证
                options.SuppressModelStateInvalidFilter = true;
                //格式化验证信息
                options.InvalidModelStateResponseFactory = (context) =>
                {
                    var problemDetails = new ProblemDetails
                    {
                        Detail = context.ModelState.GetValidationSummary("<br>"),
                        Title = "参数错误",
                        Status = (int)HttpStatusCode.BadRequest,
                        Type = "https://httpstatuses.com/400",
                        Instance = context.HttpContext.Request.Path
                    };

                    return new ObjectResult(problemDetails)
                    {
                        StatusCode = problemDetails.Status
                    };
                };
            });
        }

        /// <summary>
        /// 注册身份认证组件
        /// </summary>
        protected virtual void AddAuthentication()
        {
            Services.AddScoped<LoginUser>();
            var jwtConfig = Configuration.GetJWTSection().Get<JwtConfig>();

            Services.AddAuthentication()
            .AddJwtBearer(options =>
            {
                //校验配置
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtConfig.Issuer,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SymmetricSecurityKey)),
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(jwtConfig.ClockSkew),
                };
                //校验后事件
                options.Events = new JwtBearerEvents
                {
                    //接受到消息时调用
                    OnMessageReceived = context => Task.CompletedTask
                    ,
                    //在Token验证通过后调用
                    OnTokenValidated = context =>
                    {
                        var userId = long.Parse(context.Principal.Claims.FirstOrDefault(x => x.Type.Contains(JwtRegisteredClaimNames.NameId)).Value);
                        var loginUser= Services.BuildServiceProvider().GetService<LoginUser>();
                        loginUser.Id=userId;
                        return Task.CompletedTask ;
                    }
                     ,
                    //认证失败时调用
                    OnAuthenticationFailed = context =>
                    {
                        //如果是过期，在http heard中加入act参数
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            context.Response.Headers.Add("act", "expired");
                        return Task.CompletedTask;
                    }
                    ,
                    //未授权时调用
                    OnChallenge = context => Task.CompletedTask
                };
            });

            //因为获取声明的方式默认是走微软定义的一套映射方式
            //如果我们想要走JWT映射声明，那么我们需要将默认映射方式给移除掉
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        }

        /// <summary>
        /// 注册授权组件
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        protected virtual void AddAuthorization<THandler>()
            where THandler : AbstractPermissionHandler
        {
            Services.AddScoped<IAuthorizationHandler, THandler>();
            Services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthorizePolicy.Default, policy =>
                {
                    policy.Requirements.Add(new PermissionRequirement());
                });
            });
        }

        /// <summary>
        /// 注册跨域组件
        /// </summary>
        protected virtual void AddCors()
        {
            Services.AddCors(options =>
            {
                var _corsHosts = Configuration.GetAllowCorsHosts().Split(",", StringSplitOptions.RemoveEmptyEntries);
                options.AddPolicy(ServiceInfo.CorsPolicy, policy =>
                {
                    policy.WithOrigins(_corsHosts)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });
        }

        /// <summary>
        /// 注册swagger组件
        /// </summary>
        protected virtual void AddSwaggerGen()
        {
            var openApiInfo = new OpenApiInfo { Title = ServiceInfo.ShortName, Version = ServiceInfo.Version };

            Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(openApiInfo.Version, openApiInfo);

                // 采用bearer token认证
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });
                //设置全局认证
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{ServiceInfo.AssemblyName}.xml"));
                var contractXml = Path.Combine(AppContext.BaseDirectory,
                    $"{ServiceInfo.AssemblyName.Replace(".WebApi", "")}.Application.Contract.xml");
                if (File.Exists(contractXml))
                {
                    c.IncludeXmlComments(contractXml);
                }
                
            });

            Services.AddFluentValidationRulesToSwagger();
        }
        private static List<string> GetNames(ConfigurationSectionGroup configSectionGroup)
        {
            var names = new List<string>();

            foreach (ConfigurationSectionGroup csg in configSectionGroup.SectionGroups)
                names.AddRange(GetNames(csg));

            foreach (ConfigurationSection cs in configSectionGroup.Sections)
                names.Add(configSectionGroup.SectionGroupName + "/" + cs.SectionInformation);

            return names;
        }
        /// <summary>
        /// 注册健康监测组件
        /// </summary>
        protected virtual void AddHealthChecks()
        {
            var redisConfig = Configuration.GetRedisSection().Get<RedisConfig>();
            Services.AddHealthChecks().AddRedis(redisConfig.dbconfig.ConnectionString);
        }

        /// <summary>
        /// 注册 MiniProfiler 组件
        /// </summary>
        protected virtual void AddMiniProfiler()
        {
            Services.AddMiniProfiler(options => options.RouteBasePath = $"/{ServiceInfo.ShortName}/profiler").AddEntityFramework();
        }

        /// <summary>
        /// 注册Application层服务
        /// </summary>
        protected virtual void AddApplicationServices()
        {
            
            var appAssembly = ServiceInfo.GetApplicationAssembly();
            if (appAssembly is not null)
            {
                var modelAbstractTypes = appAssembly.GetExportedTypes()
                    .Where(m => m.IsAssignableTo(typeof(IAppService)) && m.IsInterface)
                    .ToArray();
                foreach(var modelAbstractType in modelAbstractTypes)
                {
                    var modelType = appAssembly.GetExportedTypes()
                    .FirstOrDefault(m => m.IsAssignableTo(modelAbstractType) && m.IsNotAbstractClass(true));
                    if (modelType != null)
                    {
                        Services.AddScoped(modelAbstractType,modelType);
                    }
                }
            }
        }
    }
}
