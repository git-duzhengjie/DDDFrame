﻿
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Microsoft.AspNetCore.Authorization
{
    /// <summary>
    /// 权限特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class PermissionAttribute : AuthorizeAttribute
    {
        public const string JwtWithBasicSchemes = $"{JwtBearerDefaults.AuthenticationScheme}";

        public string[] Codes { get; set; }

        public PermissionAttribute(string code, string schemes = JwtBearerDefaults.AuthenticationScheme)
            : this(new string[] { code }, schemes)
        {
        }

        public PermissionAttribute(string[] codes, string schemes = JwtBearerDefaults.AuthenticationScheme)
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
