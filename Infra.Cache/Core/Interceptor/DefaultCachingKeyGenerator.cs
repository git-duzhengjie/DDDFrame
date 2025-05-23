﻿
using Infra.Cache.Interceptor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Louge.Infra.Core.Interceptor
{
    /// <summary>
    /// 默认缓存Key生成器
    /// </summary>
    public class DefaultCachingKeyGenerator : ICachingKeyGenerator
    {
        private const char LinkChar = ':';

        //public string GetCacheKey(MethodInfo methodInfo, object[] args, string prefix)
        //{
        //    var methodArguments = args?.Any() == true
        //                              ? args.Select(ParameterCacheKeys.GenerateCacheKey)
        //                              : new[] { "0" };
        //    return GenerateCacheKey(methodInfo, prefix, methodArguments);
        //}

        public string GetCacheKey(MethodInfo methodInfo, object[] args, string prefix)
        {
            IEnumerable<string> methodArguments = new[] { "0" };
            if (args?.Any() == true)
            {
                var cacheParams = methodInfo.GetParameters().Where(x => x.GetCustomAttribute<CachingParamAttribute>() != null)
                                                                                .Select(x => x.Position);

                if (cacheParams?.Any() == true)
                    methodArguments = args.Where(x => cacheParams.Contains(Array.IndexOf(args, x)))
                                                              .Select(ParameterCacheKeys.GenerateCacheKey);
                else
                    methodArguments = args.Select(ParameterCacheKeys.GenerateCacheKey);
            }
            return GenerateCacheKey(methodInfo, prefix, methodArguments);
        }

        public string[] GetCacheKeys(MethodInfo methodInfo, object[] args, string prefix)
        {
            var cacheKeys = new List<string>();
            if (args?.Any() == true && args[0].GetType().IsArray)
            {
                foreach (var arg0 in (Array)args[0])
                {
                    var cloneArgs = (object[])args.Clone();
                    cloneArgs[0] = arg0;
                    cacheKeys.Add(GetCacheKey(methodInfo, cloneArgs, prefix));
                }
            }
            else
            {
                cacheKeys.Add(GetCacheKey(methodInfo, args, prefix));
            }

            return cacheKeys.ToArray();
        }

        public string GetCacheKeyPrefix(MethodInfo methodInfo, string prefix)
        {
            if (!string.IsNullOrWhiteSpace(prefix)) return $"{prefix}{LinkChar}";

            var typeName = methodInfo.DeclaringType?.Name;
            var methodName = methodInfo.Name;

            return $"{typeName}{LinkChar}{methodName}{LinkChar}";
        }

        private string GenerateCacheKey(MethodInfo methodInfo, string prefix, IEnumerable<string> parameters)
        {
            var cacheKeyPrefix = GetCacheKeyPrefix(methodInfo, prefix);

            var builder = new StringBuilder();
            builder.Append(cacheKeyPrefix);
            builder.Append(string.Join(LinkChar.ToString(), parameters));
            return builder.ToString();
        }
    }
}
