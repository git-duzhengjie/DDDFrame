
using Newtonsoft.Json;

namespace Microsoft.EntityFrameworkCore.Storage.ValueConversion
{
    public class JsonValueConverter<T> : ValueConverter<T, string>
    {
        public JsonValueConverter(ConverterMappingHints hints = default) :
          base(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<T>(v), hints)
        { }
    }
}
