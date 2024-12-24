using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Core.Network
{
    public class AppErr
    {
        public string type { get; set; }

        public string title { get; set; }

        public int status { get; set; }

        public string detail { get; set; }
    }
}
