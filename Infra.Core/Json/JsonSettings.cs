using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Infra.Core.Json
{
    public class JsonSettings
    {
        public static JsonSerializerSettings JsonSerializerSettings => new JsonSerializerSettings
        {
            SerializationBinder = new LougeJsonTypeBinder(),
            TypeNameHandling = TypeNameHandling.Objects,
            DefaultValueHandling = DefaultValueHandling.Populate,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            ContractResolver=WritablePropertiesOnlyResolver.Instance,
            MaxDepth = 99999999,
            TraceWriter = new MemoryTraceWriter(),
            Error = (s, e) =>
            {
                var ss = ((JsonSerializer)s).TraceWriter.ToString();
            },
        };

        public static void SetJsonSerializerSettings(JsonSerializerSettings jsonSerializerSettings)
        {
            jsonSerializerSettings.SerializationBinder = new LougeJsonTypeBinder();
            jsonSerializerSettings.TypeNameHandling = TypeNameHandling.Objects;
            jsonSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            jsonSerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            jsonSerializerSettings.ObjectCreationHandling = ObjectCreationHandling.Replace;
            jsonSerializerSettings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
            jsonSerializerSettings.MaxDepth = 99999999;
            jsonSerializerSettings.ContractResolver = WritablePropertiesOnlyResolver.Instance;
            jsonSerializerSettings.TraceWriter = new MemoryTraceWriter();
            jsonSerializerSettings.Error = (s, e) =>
            {
                var ss = ((JsonSerializer)s).TraceWriter.ToString();
            };
        }
    }
}
