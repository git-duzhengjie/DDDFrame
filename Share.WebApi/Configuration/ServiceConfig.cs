using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.WebApi.Configuration
{
    public class ServiceConfig
    {
        public string ServiceName { get; set; }

        public bool Infusionable { get; set; }

        public bool Important { get; set; }

        public bool IfSubscribe { get; set; }

        public string Protocol { get; set; }

        public string IpAddress { get; set; }

        public int? MaxRetryAttempts { get; set; }

        public int Port { get; set; }

        public int Interval { get; set; } = 15000;


        public string GatewayUrl { get; set; }
    }
}
