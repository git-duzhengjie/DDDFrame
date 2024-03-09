using Newtonsoft.Json;
using System;

namespace Infra.Core.Json
{
    //public class GeometryBaseConverter : JsonConverter
    //{
    //    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    //    {
    //        var json = reader.Value?.ToString();
    //        if (json == null)
    //            return null;
    //        return (GeometryBase)CommonObject.FromJSON(json);
    //    }

    //    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    //    {
    //        if (value is GeometryBase geometry)
    //        {
    //            writer.WriteValue(geometry.ToJSON(new Rhino.FileIO.SerializationOptions()));
    //        }
    //    }

    //    public override bool CanConvert(Type objectType)
    //    {
    //        return typeof(GeometryBase).IsAssignableFrom(objectType);
    //    }
    //}
}
