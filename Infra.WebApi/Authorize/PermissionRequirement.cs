
namespace Microsoft.AspNetCore.Authorization
{
    /// <summary>
    /// 授权请求
    /// </summary>
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Name { get; init; }

        public PermissionRequirement()
        {
        }

        public PermissionRequirement(string name) => Name = name;
    }
}
