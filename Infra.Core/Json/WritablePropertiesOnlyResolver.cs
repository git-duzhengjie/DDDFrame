using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Dynamic;
using System.Reflection;

namespace Infra.Core.Json
{
    public class WritablePropertiesOnlyResolver : CamelCasePropertyNamesContractResolver
    {
        private static bool isGetAllProperties = false;
        public static WritablePropertiesOnlyResolver Instance { get; } = new WritablePropertiesOnlyResolver();

        public IDictionary<Type, (JsonContract FullProperties, JsonContract ForeignKeyProperties)> propertiesCacheMap 
            = new Dictionary<Type, (JsonContract FullProperties, JsonContract ForeignKeyProperties)>();
        private WritablePropertiesOnlyResolver()
        {
        }

        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            var typeInfo = objectType.GetTypeInfo();
            if (typeInfo.IsGenericType && !typeInfo.IsGenericTypeDefinition)
            {
                var jsonConverterAttribute = typeInfo.GetCustomAttribute<JsonConverterAttribute>();
                if (jsonConverterAttribute != null && jsonConverterAttribute.ConverterType.GetTypeInfo().IsGenericTypeDefinition)
                {
                    Type t = jsonConverterAttribute.ConverterType.MakeGenericType(typeInfo.GenericTypeArguments);
                    object[] parameters = jsonConverterAttribute.ConverterParameters;
                    return (JsonConverter)Activator.CreateInstance(t, parameters);
                }
            }
            return base.ResolveContractConverter(objectType);
        }
        public override JsonContract ResolveContract(Type type)
        {
            if (typeof(IDynamicMetaObjectProvider).IsAssignableFrom(type))
            {
                var c = base.ResolveContract(type);
                return c;
            }
            propertiesCacheMap.TryGetValue(type, out var cacheMap);
            if (cacheMap.FullProperties == null)
            {
                lock (propertiesCacheMap)
                {
                    propertiesCacheMap.TryGetValue(type, out cacheMap);
                    if (cacheMap.FullProperties == null)
                    {
                        isGetAllProperties = true;
                        var fullProperties = CreateContract(type);
                        isGetAllProperties = false;
                        var foreignKeyProperties = CreateContract(type);
                        propertiesCacheMap.Add(type, (fullProperties, foreignKeyProperties));
                    }
                }
            }
            propertiesCacheMap.TryGetValue(type, out cacheMap);
            return cacheMap.FullProperties;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
            if (type.Namespace == null)
            {
                return props;
            }

            if (type.Namespace.StartsWith("Louge"))
            {
                var propertyConfigs = props.Select(x => new
                {
                    x.Writable,
                    x.PropertyType,
                    Property = x
                }).ToList();
                propertyConfigs = propertyConfigs.Where(p => p.Writable).ToList();
                if (!isGetAllProperties)
                {
                    propertyConfigs = propertyConfigs.Where(x =>  x.PropertyType.IsValueType || x.PropertyType == typeof(string) ).ToList();
                }
                props = propertyConfigs.Select(x => x.Property).ToList();
            }
            return props;
        }
    }
}
