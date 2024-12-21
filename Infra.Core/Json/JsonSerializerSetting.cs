using System.Text.Json;

namespace Infra.Core.Json
{
    public static class JsonSerializerSetting
    {
        public static JsonSerializerOptions JsonSerializerOptions => new JsonSerializerOptions
        {
            WriteIndented = true,
        };

        public static void SetJsonSerializerOptions(this JsonSerializerOptions jsonSerializerSettings)
        {
           jsonSerializerSettings.WriteIndented = true;
        }
    }
}
