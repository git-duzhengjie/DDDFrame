
using System.Text.Json;

namespace Microsoft.EntityFrameworkCore.Storage.ValueConversion
{
    public static class Converter{
        public static T Deserialize<T>(string v)
        {
            return JsonSerializer.Deserialize<T>(v);
        }

        public static string Serialize<T>(T v)
        {
            return JsonSerializer.Serialize(v);
        }
    }
    public class JsonValueConverter<T> : ValueConverter<T, string> where T : class
    {
        public JsonValueConverter(ConverterMappingHints hints = default) :
          base(v => Converter.Serialize(v), v => Converter.Deserialize<T>(v), hints)
        { }

        
    }
}
