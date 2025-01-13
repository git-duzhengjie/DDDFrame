using Infra.Core.Models;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace Infra.WebApi.Extentions
{
    public static class ContextExtension
    {
        public static LoginInfo GetLoginInfo(this IHttpContextAccessor httpContextAccessor)
        {
            var claims = httpContextAccessor.HttpContext.User.Claims;
            Debug.Assert(claims != null);
            var userClaim = claims.FirstOrDefault(x => x?.Type == "UserId");
            Debug.Assert(userClaim != null);
            var value = userClaim.Value;
            Debug.Assert(value != null);
            var userId = long.Parse(value);
            var systemInfoClaim = claims.FirstOrDefault(x => x.Type == "SystemInfo");
            Debug.Assert(systemInfoClaim != null);
            var systemInfo = systemInfoClaim.Value;
            var loginUser = new LoginInfo() { SystemInfo = systemInfo, UserId = userId };
            return loginUser;
        }
    }
}
