
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace Microsoft.AspNetCore.Authorization
{
    /// <summary>
    /// 授权处理基类
    /// </summary>
    public abstract class AbstractPermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User.Identity.IsAuthenticated && context.Resource is HttpContext httpContext)
            {
                var authHeader = httpContext.Request.Headers["Authorization"].ToString();
                if (authHeader != null)
                {
                    context.Succeed(requirement);
                    return;
                }

                var userId = Int64.Parse(context.User.Claims.First(x => x.Type == JwtRegisteredClaimNames.NameId).Value);
                var validationVersion = context.User.Claims.FirstOrDefault(x => x.Type == "version")?.Value;
                var codes = httpContext.GetEndpoint().Metadata.GetMetadata<PermissionAttribute>().Codes;
                var result = await CheckUserPermissions(userId, codes, validationVersion);
                if (result)
                {
                    context.Succeed(requirement);
                    return;
                }
            }
            context.Fail();
        }

        protected abstract Task<bool> CheckUserPermissions(Int64 userId, IEnumerable<String> codes, String validationVersion);
    }
}
