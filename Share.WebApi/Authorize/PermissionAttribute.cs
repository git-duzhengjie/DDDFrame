
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Microsoft.AspNetCore.Authorization
{
    /// <summary>
    /// 权限特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class PermissionAttribute : AuthorizeAttribute
    {
        public const String JwtWithBasicSchemes = $"{JwtBearerDefaults.AuthenticationScheme}";

        public String[] Codes { get; set; }

        public PermissionAttribute(String code, String schemes = JwtBearerDefaults.AuthenticationScheme)
            : this(new String[] { code }, schemes)
        {
        }

        public PermissionAttribute(String[] codes, String schemes = JwtBearerDefaults.AuthenticationScheme)
        {
            Codes = codes;
            Policy = AuthorizePolicy.Default;
            if (schemes.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(schemes));
            else
                AuthenticationSchemes = schemes;
        }
    }
}
