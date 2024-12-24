using Mapster;

namespace Infra.Core.Extensions
{
    public static class MapsterExtension
    {
        public static T MapTo<T>(this object object)
        {
            return object.Adapt<T>();
        }
    }

    public class MapTo<T>
    {
        public static T GetValue(object object)
        {
            return object.MapTo<T>();
        }
    }
}
