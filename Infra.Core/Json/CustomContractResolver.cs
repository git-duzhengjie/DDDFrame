using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Infra.Core.Json
{
    public sealed class CustomContractResolver : DefaultContractResolver
    {
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
    }
}
