
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
        public const string DefaultSerializerName = "binary";

        public const string DefaultProtobufSerializerName = "proto";

        public const string DefaultJsonSerializerName = "json";

        public const string StackExchange = "redis.stackexchange";

        public const Int32 PollyTimeout = 5;
    }
}
