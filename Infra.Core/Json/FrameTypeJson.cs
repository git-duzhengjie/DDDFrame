using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Infra.Core.Json
{
    internal class FrameTypeJson
    {
        private string json;
        private JsonSerializerOptions serializerOptions;
        private Dictionary<string, Type?> objectTypeMap;
        private ILogger<FrameJson>? logger;

        public FrameTypeJson(string json, JsonSerializerOptions serializerOptions, Dictionary<string, Type?> objectTypeMap, Microsoft.Extensions.Logging.ILogger<FrameJson>? logger)
        {
            this.json = json;
            this.serializerOptions = serializerOptions;
            this.objectTypeMap=objectTypeMap;
            this.logger=logger;
        }

        internal T Deserialize<T>()
        {
            return (T)Deserialize(typeof(T));
        }

        internal object Deserialize(Type modelType)
        {
            try
            {
                JsonElement jValue = JsonSerializer.Deserialize<dynamic>(json);
                return Deserialize(modelType, jValue);
            }
            catch (Exception ex) {
                if (logger != null&&ex!=null)
                {
                    logger.LogError(ex.ToString());
                }
                else
                {
                    Console.WriteLine(ex.ToString());
                }
                
                throw;
            }
        }

        internal object Deserialize(Type modelType, JsonElement jValue)
        {
            if (modelType.IsAbstract)
            {
                switch (jValue.ValueKind)
                {
                    case JsonValueKind.Object:
                        return ParseObjectWithoutType(jValue,modelType);
                    case JsonValueKind.Array:
                        var result=new List<object>();
                        var count=jValue.GetArrayLength();
                        var geType=modelType.GetGenericArguments()[0];
                        foreach(var element in jValue.EnumerateArray()){
                            if (geType.IsAbstract)
                            {
                                result.Add(ParseObjectWithoutType(element,geType));
                            }
                            else
                            {
                                result.Add(ParseObjectWithType(geType,element));
                            }
                        }
                        var r = ConvertToType(result, geType, modelType.IsArray);
                        return r;
                    default:
                        return JsonElementToValue(jValue, modelType);
                }
            }
            else
            {
                switch (jValue.ValueKind)
                {
                    case JsonValueKind.Object:
                        return ParseObjectWithType(modelType,jValue);
                    case JsonValueKind.Array:
                        var result = new List<object>();
                        var count = jValue.GetArrayLength();
                        var geType = modelType.GetGenericArguments().FirstOrDefault()??modelType.GetElementType();
                        foreach (var element in jValue.EnumerateArray())
                        {
                            if (geType.IsValueType)
                            {
                                result.Add(JsonElementToValue(element,geType));
                            }else if (geType == typeof(string))
                            {
                                result.Add(element.GetString());
                            }
                            else
                            {
                                if (geType.IsAbstract)
                                {
                                    result.Add(ParseObjectWithoutType(element, geType));
                                }
                                else
                                {
                                    result.Add(ParseObjectWithType(geType, element));
                                }
                            }
                        }
                        var r = ConvertToType(result,geType,modelType.IsArray);
                        return r;
                    default:
                        return JsonElementToValue(jValue,modelType);
                }
            }
        }

        private object ConvertToType(List<object> result, Type geType, bool isArray)
        {
            var convertType = typeof(ListTypeConverter<>).MakeGenericType(geType);
            return convertType.InvokeMethod("Convert",result,isArray);
        }

        private object ParseObjectWithType(Type type, JsonElement jValue)
        {
            var obj = Activator.CreateInstance(type);
            SetValue(obj, jValue);
            return obj;
        }

        private object ParseObjectWithoutType(JsonElement jValue, Type modelType)
        {
            var objectName = jValue.GetProperty("ObjectName").ToString();
            if (objectName.IsNotNullOrEmpty() && objectTypeMap.TryGetValue(objectName, out var type))
            {
                var obj = Activator.CreateInstance(type);
                SetValue(obj, jValue);
                return obj;
            }
            throw new Exception($"无法解析{modelType.Name}");
        }

        private void SetValue(object? obj, JsonElement je)
        {
            var type=obj.GetType();
            var properties=type.GetProperties();
            foreach (var property in properties) {
                if (property.CanWrite&& TryGetValue(property.Name,je,out var value))
                {
                    if (property.PropertyType.IsValueType)
                    {
                        property.SetValue(obj, JsonElementToValue(value,property.PropertyType));
                    }
                    else
                    {
                        property.SetValue(obj, Deserialize(property.PropertyType, value));
                    }
                }
                
            }
            var fields=type.GetFields();
            foreach (var field in fields) { 
                if(TryGetValue(field.Name, je, out var value))
                {
                    if (field.FieldType.IsValueType)
                    {
                        field.SetValue(obj, JsonElementToValue(value,field.FieldType));
                    }
                    else
                    {
                        field.SetValue(obj,Deserialize(field.FieldType,value));
                    }
                }
            }
        }

        private object? JsonElementToValue(JsonElement value, Type propertyType)
        {
            if (value.ValueKind == JsonValueKind.String && propertyType != typeof(string))
            {
                if (propertyType.IsEnum)
                {
                    return Enum.Parse(propertyType, value.GetString());
                }else if (propertyType==typeof(int))
                {
                    return int.Parse(value.GetString());
                }else if (propertyType == typeof(float))
                {
                    return float.Parse(value.GetString());
                }else if (propertyType == typeof(double))
                {
                    return double.Parse(value.GetString());
                }
            }
            if (propertyType == typeof(string))
            {
                return value.GetString();
            }
            return value.Deserialize(propertyType);
        }

        private static bool TryGetValue(string name, JsonElement je, out JsonElement value)
        {
            if(je.TryGetProperty(name, out value))
            {
                return true;
            }
            name= string.Concat(name[..1].ToLower(), name.AsSpan(1));
            
            if(je.TryGetProperty(name,out value)){
                return true;
            }
            value = default;
            return false;
        }
    }
}