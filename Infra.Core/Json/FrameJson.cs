using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Xml.Linq;
using Infra.Core.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using UniversalRPC.Serialization;
using static System.Text.Encoding;
using Microsoft.Extensions.Logging;
using System.Buffers;
using UniversalRPC.Extensions;
using UniversalRpc.Extensions;
using System.Collections.Concurrent;
namespace Infra.Core.Json
{
    public class FrameJson : IOutputFormatter, IInputFormatter, ISerialize
    {
        private ConcurrentDictionary<string, Type?> objectTypeMap = null;
        public FrameJson()
        {
            RegisterProvider(CodePagesEncodingProvider.Instance);
            GenerateTypeMap();

        }
        public void GenerateTypeMap()
        {
            if (objectTypeMap == null)
            {
                objectTypeMap = [];
                var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(x=>x.IsNotOut())
                    .ToArray();
                foreach (var assembly in assemblies)
                {
                    try
                    {
                        var types = assembly.GetExportedTypes()
                        .Where(x => x.IsNotAbstractClass(true))
                        .ToArray();
                        foreach (var type in types)
                        {
                            var interfaces = type.GetInterfaces();
                            if (interfaces.Contains(typeof(IObject)))
                            {
                                IObject instance;
                                if (type.IsGenericType)
                                {
                                    var newType = type.MakeGenericType(typeof(int));
                                    instance = Activator.CreateInstance(newType) as IObject;
                                }
                                else
                                {
                                    instance = Activator.CreateInstance(type) as IObject;
                                }

                                if (!objectTypeMap.TryAdd(instance.ObjectName, type))
                                {
                                    throw new Exception($"{instance.ObjectName}该对象名已经存在");
                                }
                            }
                        }
                    }
                    catch
                    {
                        continue;
                    }
                    
                }

            }
        }
        public static string Serialize<T>(T obj, JsonSerializerOptions serializerOptions = null) where T : IObject
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, new Newtonsoft.Json.JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            });
        }

        private bool IsIObject(string json)
        {
            return json.Contains("ObjectName") || json.Contains("objectName");
        }
        public T Desialize<T>(string json, JsonSerializerOptions serializerOptions = null)
        {
            if (json.IsNullOrEmpty())
            {
                return default;
            }
            if (!IsIObject(json)&&!json.Contains("$type")&&typeof(T).IsNotAbstractClass(true))
            {
                return JsonSerializer.Deserialize<T>(json, serializerOptions);
            }
            else
            {
                return new FrameTypeJson(json,serializerOptions, objectTypeMap,null).Deserialize<T>();
            }
            
        }

        public object Desialize(string json, Type modelType, ILogger<FrameJson>? logger, JsonSerializerOptions serializerOptions = null)
        {
            if (json.IsNullOrEmpty())
            {
                return null;
            }
            if (!IsIObject(json)&& !json.Contains("$type")&&modelType.IsNotAbstractClass(true))
            {
                return JsonSerializer.Deserialize(json,modelType, serializerOptions);
            }
            else
            {
                return new FrameTypeJson(json, serializerOptions, objectTypeMap,logger).Deserialize(modelType);
            }
        }

        public bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            return true;
        }

        public async Task WriteAsync(OutputFormatterWriteContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            _ = context.HttpContext.RequestServices.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
            var response = context.HttpContext.Response;
            var value = context.Object;
            var content = Newtonsoft.Json.JsonConvert.SerializeObject(value,new Newtonsoft.Json.JsonSerializerSettings
            {
                ContractResolver=new CamelCasePropertyNamesContractResolver(),
            });
            await response.WriteAsync(content);
        }

        public bool CanRead(InputFormatterContext context)
        {
            return true;
        }

        public async Task<InputFormatterResult> ReadAsync(InputFormatterContext context)
        {
            var input = context.HttpContext.Request.BodyReader;
            var str = await GetResultAsync(context);
            var jsonOptions = context.HttpContext.RequestServices.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
            var logger = context.HttpContext.RequestServices.GetService<ILogger<FrameJson>>();
            return await InputFormatterResult.SuccessAsync(Desialize(str, context.ModelType, logger,jsonOptions));

        }

        private static async Task<string> GetResultAsync(InputFormatterContext context)
        {
            using StreamReader sr = new(context.HttpContext.Request.Body);
            return await sr.ReadToEndAsync();
        }

        public string Serialize(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, new Newtonsoft.Json.JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            });
        }

        public T Deserialize<T>(string str)
        {
            return Desialize<T>(str, JsonSerializerSetting.JsonSerializerOptions);
        }
    }

    public class DynamicConvert<T> where T : class
    {
        public static T GetValue(dynamic data)
        {
            return data as T;
        }

    }
}
