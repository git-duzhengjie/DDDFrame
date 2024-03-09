
using System;

namespace Infra.WebApi.Consts.Caching.Common
{
    public class CachingConsts
    {
        protected CachingConsts()
        { }

        public const String LinkChar = ":";

        public const Int32 OneYear = 365 * 24 * 60 * 60;
        public const Int32 OneMonth = 30 * 24 * 60 * 60;
        public const Int32 OneWeek = 7 * 24 * 60 * 60;
        public const Int32 OneDay = 24 * 60 * 60;
        public const Int32 OneHour = 60 * 60;
        public const Int32 OneMinute = 60;

        public static String Prefix
        {
            get
            {
                var prefix = "louge";
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ToLower();
                switch (environment)
                {
                    case "development":
                        prefix = "_dev";
                        break;
                    case "test":
                        prefix = "_test";
                        break;
                    case "staging":
                        prefix = "_staging";
                        break;
                    case "production":
                        prefix = "_prod";
                        break;
                    default:
                        prefix = "_unknown";
                        break;
                }
                return prefix;
            }
        }

        public static String WorkerIdSortedSetCacheKey => $"louge:{{0}}:workids";
    }
}
