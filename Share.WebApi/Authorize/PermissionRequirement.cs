
namespace Microsoft.AspNetCore.Authorization
{
    /// <summary>
    /// 授权请求
    /// </summary>
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public String Name { get; init; }

        public PermissionRequirement()
        {
        }

        public PermissionRequirement(String name) => Name = name;
    }
}
