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
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace Infra.Core.Json
{
    public class FrameJson : IOutputFormatter, IInputFormatter, ISerialize
    {
        private static Dictionary<string, Type?> objectTypeMap = null;


        public FrameJson()
        {
            RegisterProvider(CodePagesEncodingProvider.Instance);
            if (objectTypeMap == null)
            {
                objectTypeMap = new Dictionary<string, Type?>();
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    var types = assembly.GetExportedTypes()
                        .Where(x => x.IsNotAbstractClass(true))
                        .ToArray();
                    foreach (var type in types)
                    {
                        var interfaces = type.GetInterfaces();
                        if (interfaces.Contains(typeof(IObject)))
                        {
                            var instance = Activator.CreateInstance(type) as IObject;

                            if (!objectTypeMap.ContainsKey(instance.ObjectName))
                            {
                                objectTypeMap.Add(instance.ObjectName, type);
                            }
                            else
                            {
                                throw new Exception($"{instance.ObjectName}该对象名已经存在");
                            }
                        }
                    }
                }

            }

        }
        public static string Serialize<T>(T obj, JsonSerializerOptions serializerOptions = null) where T : IObject
        {
            return JsonSerializer.Serialize(obj, serializerOptions);
        }

        public static T Desialize<T>(string json, JsonSerializerOptions serializerOptions = null)
        {
            if (!json.Contains("ObjectName")&&!json.Contains("$type"))
            {
                return JsonSerializer.Deserialize<T>(json, serializerOptions);
            }
            else
            {
                return new FrameTypeJson(json,serializerOptions, objectTypeMap).Deserialize<T>();
            }
            
        }

        public static object Desialize(string json, Type modelType, JsonSerializerOptions serializerOptions = null)
        {
            if (json.IsNullOrEmpty())
            {
                return json;
            }
            if (!json.Contains("ObjectName") && !json.Contains("$type"))
            {
                return JsonSerializer.Deserialize(json,modelType, serializerOptions);
            }
            else
            {
                return new FrameTypeJson(json, serializerOptions, objectTypeMap).Deserialize(modelType);
            }
        }

        public bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            return true;
        }

        public async Task WriteAsync(OutputFormatterWriteContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            var jsonOptions = context.HttpContext.RequestServices.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
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
            var readResult = await input.ReadAsync();
            var jsonOptions = context.HttpContext.RequestServices.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
            var str = Encoding.UTF8.GetString(readResult.Buffer);
            return await InputFormatterResult.SuccessAsync(Desialize(str, context.ModelType, jsonOptions));
        }

        public string Serialize(object obj)
        {
            return JsonSerializer.Serialize(obj, JsonSerializerSetting.JsonSerializerOptions);
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
