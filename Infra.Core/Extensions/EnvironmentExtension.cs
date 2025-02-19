using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Core.Extensions
{
    public static class EnvironmentExtension
    {
        public static bool IsWin(this OperatingSystem os)
        {
            PlatformID platform = os.Platform;
            switch (platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    return true;
            }
            return false;
        }
    }
}
