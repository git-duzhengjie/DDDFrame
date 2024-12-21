using Mapster;

namespace Infra.Core.Extensions
{
    public static class MapsterExtension
    {
        public static T MapTo<T>(this object obj)
        {
            return obj.Adapt<T>();
        }
    }

    public class MapTo<T>
    {
        public static T GetValue(object obj)
        {
            return obj.MapTo<T>();
        }
    }
}
