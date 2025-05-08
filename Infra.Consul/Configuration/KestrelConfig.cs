
using System;
using System.Collections.Generic;

namespace Infra.Consul.Configuration
{
    /// <summary>
    /// Kestrel配置
    /// </summary>
    public class KestrelConfig
    {
        #region <属性>

        public IDictionary<string, Endpoint> Endpoints { get; set; }

        #endregion <属性>

        #region <构造方法和析构方法>

        public KestrelConfig()
        {
            Endpoints = new Dictionary<string, Endpoint>();
        }

        #endregion <构造方法和析构方法>

        #region <内部类>

        public class Endpoint
        {
            public Endpoint()
            {
                if (string.IsNullOrWhiteSpace(Protocols))
                    Protocols = "Http1AndHttp2";
            }

            /// <summary>
            /// Url地址
            /// </summary>
            public string Url { get; set; }

            /// <summary>
            /// 协议
            /// </summary>
            public string Protocols { get; set; }
        }

        #endregion <内部类>
    }
}
