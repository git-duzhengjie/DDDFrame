namespace Infra.Core.Json
{
    public class ListTypeConverter<T>
    {
        public static IEnumerable<T> Convert(IEnumerable<object> objects,bool isArray)
        {
            if (isArray)
            {
                return objects.Cast<T>().ToArray();
            }
            return objects.Cast<T>().ToList();
        }
    }
}
