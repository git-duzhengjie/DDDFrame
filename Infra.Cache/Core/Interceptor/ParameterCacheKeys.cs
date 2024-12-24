
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Louge.Infra.Core.Interceptor
{
    public static class ParameterCacheKeys
    {
        public static String GenerateCacheKey(object parameter)
        {
            if (parameter == null) return String.Empty;
            if (parameter is ICachable cachable) return cachable.CacheKey;
            if (parameter is String key) return key;
            if (parameter is DateTime dateTime) return dateTime.ToString("O");
            if (parameter is DateTimeOffset dateTimeOffset) return dateTimeOffset.ToString("O");
            if (parameter is IEnumerable enumerable) return GenerateCacheKey(enumerable.Cast<object>());
            return parameter.ToString();
        }

        private static String GenerateCacheKey(List<object> parameter)
        {
            if (parameter == null) return String.Empty;
            return "[" + String.Join(",", parameter) + "]";
        }
    }
}
