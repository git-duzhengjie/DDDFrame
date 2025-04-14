using Infra.Core.Models;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace Infra.WebApi.Extentions
{
    public static class ContextExtension
    {
        public static LoginInfo GetLoginInfo(this IHttpContextAccessor httpContextAccessor)
        {
            try
            {
                var claims = httpContextAccessor.HttpContext.User.Claims;
                var userClaim = claims.FirstOrDefault(x => x?.Type == "UserId");
                if (userClaim == null)
                {
                    return null;
                }
                var value = userClaim.Value;
                var userId = long.Parse(value);
                var systemInfoClaim = claims.FirstOrDefault(x => x.Type == "SystemInfo");
                string systemInfo = null;
                if (systemInfoClaim != null)
                {
                    systemInfo = systemInfoClaim.Value;
                }
                var loginUser = new LoginInfo() { SystemInfo = systemInfo, UserId = userId };
                return loginUser;
            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
    }
}
