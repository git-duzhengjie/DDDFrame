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
        private static Dictionary<int,Type?> objectTypeMap=null;


        public FrameJson() {
            if (objectTypeMap == null)
            {
                objectTypeMap = new Dictionary<int, Type?>();
                var assembly = Assembly.GetExecutingAssembly();
                var types = assembly.GetExportedTypes();
                foreach (var type in types)
                {
                    var interfaces = type.GetInterfaces();
                    if (interfaces.Contains(typeof(Iobject)))
                    {
                        var instance = Activator.CreateInstance(type) as Iobject;

                        if (!objectTypeMap.ContainsKey(instance.objectType))
                        {
                            objectTypeMap.Add(instance.objectType, type);
                        }
                        else
                        {
                            throw new Exception($"{instance.objectName}该对象名已经存在");
                        }
                    }
                }
            }
            
        }
        public static string Serialize<T>(T object,JsonSerializerOptions serializerOptions=null) where T : Iobject
        {
            return JsonSerializer.Serialize(object, serializerOptions);
        }

        public static T Desialize<T>(string json,JsonSerializerOptions serializerOptions=null)
            where T :class
        {
            var object= JsonSerializer.Deserialize<dynamic>(json,serializerOptions);
            if (object.objectName == null)
            {
                return object as T;
            }
            if(objectTypeMap.TryGetValue((int)object.objectType,out var type))
            {
                var convertType = typeof(DynamicConvert<>).MakeGenericType(type);
                return convertType.InvokeMethod("GetValue", [object]) as T;
            }
            throw new Exception($"没有找到类型{object.objectName}");
        }

        public static object Desialize(string json, Type modelType, JsonSerializerOptions serializerOptions = null)
        {
            var object = JsonSerializer.Deserialize<dynamic>(json);
            if (object.objectName == null)
            {
                var convertType = typeof(DynamicConvert<>).MakeGenericType(modelType);
                return convertType.InvokeMethod("GetValue", [object]);
            }
            if (objectTypeMap.TryGetValue((int)object.objectType, out var type))
            {
                var convertType = typeof(DynamicConvert<>).MakeGenericType(type);
                return convertType.InvokeMethod("GetValue", [object]);
            }
            throw new Exception($"没有找到类型{object.objectName}");
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

            var value = context.object;
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
