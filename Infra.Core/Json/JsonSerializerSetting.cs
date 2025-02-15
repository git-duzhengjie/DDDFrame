using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
namespace Infra.Core.Json
{
    public static class JsonSerializerSetting
    {
        public static JsonSerializerOptions JsonSerializerOptions => new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy= JsonNamingPolicy.CamelCase
        };

        public static void SetJsonSerializerOptions(this JsonSerializerOptions jsonSerializerSettings)
        {
            jsonSerializerSettings.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            jsonSerializerSettings.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        }
    }
}
