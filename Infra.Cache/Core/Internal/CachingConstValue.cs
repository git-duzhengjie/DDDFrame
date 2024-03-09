
using System;

namespace Infra.Cache.Core
{
    /// <summary>
    /// Infra.Cache const value.
    /// </summary>
    public class CachingConstValue
    {
        /// <summary>
        /// The config section.
        /// </summary>
        //public const string ConfigSection = "Infra.Cache";
        /// <summary>
        /// The default name of the serializer.
        /// </summary>
        public const String DefaultSerializerName = "binary";

        public const String DefaultProtobufSerializerName = "proto";

        public const String DefaultJsonSerializerName = "json";

        public const String StackExchange = "redis.stackexchange";

        public const Int32 PollyTimeout = 5;
    }
}
