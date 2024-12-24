
using System;
using System.Collections.Generic;

namespace Infra.Cache.Core.Diagnostics
{
    public class BeforeSetRequestEventData : EventData
    {
        public BeforeSetRequestEventData(String cacheType, String name, String operation, IDictionary<String, object> dict, System.TimeSpan expiration)
            : base(cacheType, name, operation)
        {
            this.Dict = dict;
            this.Expiration = expiration;
        }

        public IDictionary<String, object> Dict { get; set; }

        public TimeSpan Expiration { get; set; }
    }
}
