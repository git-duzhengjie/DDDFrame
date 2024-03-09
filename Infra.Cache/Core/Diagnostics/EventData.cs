
using System;

namespace Infra.Cache.Core.Diagnostics
{
    public class EventData
    {
        public EventData(String cacheType, String name, String operation)
        {
            this.CacheType = cacheType;
            this.Name = name;
            this.Operation = operation;
        }

        public String CacheType { get; set; }

        public String Name { get; set; }

        public String Operation { get; set; }
    }
}
