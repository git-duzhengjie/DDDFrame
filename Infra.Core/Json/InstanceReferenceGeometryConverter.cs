
namespace Infra.Core.Json
{
    ///// <summary>
    ///// 转换器
    ///// </summary>
    //public class InstanceReferenceGeometryConverter : JsonConverter<InstanceReferenceGeometry>
    //{
    //    public override InstanceReferenceGeometry ReadJson(JsonReader reader, Type objectType, InstanceReferenceGeometry existingValue, bool hasExistingValue, JsonSerializer serializer)
    //    {
    //        var json = reader.Value?.ToString();
    //        if (json == null)
    //            return null;
    //        return (InstanceReferenceGeometry)CommonObject.FromJSON(json);
    //    }

    //    public override void WriteJson(JsonWriter writer, InstanceReferenceGeometry value, JsonSerializer serializer)
    //    {
    //        writer.WriteValue(value.ToJSON(new Rhino.FileIO.SerializationOptions()));
    //    }
    //}
}
