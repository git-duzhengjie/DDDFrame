using System.Reflection;
using System.Text;
using System.Text.Json;
using Infra.Core.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
namespace Infra.Core.Json
{
    public class FrameJson:IOutputFormatter,IInputFormatter
    {
        private static Dictionary<string,Type?> objectTypeMap=null;


        public FrameJson() {
            if (objectTypeMap == null)
            {
                objectTypeMap = new Dictionary<string, Type?>();
                var assembly = Assembly.GetExecutingAssembly();
                var types = assembly.GetExportedTypes();
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
        public static string Serialize<T>(T obj,JsonSerializerOptions serializerOptions=null) where T : IObject
        {
            return JsonSerializer.Serialize(obj, serializerOptions);
        }

        public static T Desialize<T>(string json,JsonSerializerOptions serializerOptions=null)
            where T :class
        {
            var obj= JsonSerializer.Deserialize<dynamic>(json,serializerOptions);
            if (obj.ObjectName == null)
            {
                return obj as T;
            }
            if(objectTypeMap.TryGetValue(obj.ObjectName as string,out var type))
            {
                var convertType = typeof(DynamicConvert<>).MakeGenericType(type);
                return convertType.InvokeMethod("GetValue", [obj]) as T;
            }
            throw new Exception($"没有找到类型{obj.ObjectName}");
        }

        public static object Desialize(string json, Type modelType, JsonSerializerOptions serializerOptions = null)
        {
            var obj = JsonSerializer.Deserialize<dynamic>(json);
            if (obj.ObjectName == null)
            {
                var convertType = typeof(DynamicConvert<>).MakeGenericType(modelType);
                return convertType.InvokeMethod("GetValue", [obj]);
            }
            if (objectTypeMap.TryGetValue(obj.ObjectName as string, out var type))
            {
                var convertType = typeof(DynamicConvert<>).MakeGenericType(type);
                return convertType.InvokeMethod("GetValue", [obj]);
            }
            throw new Exception($"没有找到类型{obj.ObjectName}");
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
            await response.WriteAsync(JsonSerializer.Serialize(value, jsonOptions));
        }

        public bool CanRead(InputFormatterContext context)
        {
            return true;
        }

        public async Task<InputFormatterResult> ReadAsync(InputFormatterContext context)
        {
            var input = context.HttpContext.Request.BodyReader;
            var readResult=await input.ReadAsync();
            var jsonOptions = context.HttpContext.RequestServices.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
            var str=Encoding.UTF8.GetString(readResult.Buffer);
            return await InputFormatterResult.SuccessAsync(Desialize(str,context.ModelType,jsonOptions));
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
