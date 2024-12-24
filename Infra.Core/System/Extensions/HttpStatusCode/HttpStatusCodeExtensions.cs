
namespace System.Net
{
    /// <summary>
    /// Http状态码扩展
    /// </summary>
    public static class HttpStatusCodeExtensions
    {
        public static bool Is2xx(this HttpStatusCode _, Int32 statusCode) => statusCode >= 200 && statusCode <= 299;

        public static bool Is2xx(this HttpStatusCode statusCode) => (Int32)statusCode >= 200 && (Int32)statusCode <= 299;

        public static bool Is4xx(this HttpStatusCode _, Int32 statusCode) => statusCode >= 400 && statusCode <= 499;

        public static bool Is4xx(this HttpStatusCode statusCode) => (Int32)statusCode >= 400 && (Int32)statusCode <= 499;

        public static bool Is5xx(this HttpStatusCode _, Int32 statusCode) => statusCode >= 500 && statusCode <= 599;

        public static bool Is5xx(this HttpStatusCode statusCode) => (Int32)statusCode >= 500 && (Int32)statusCode <= 599;
    }
}
