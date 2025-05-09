﻿

using Polly.Caching;

namespace Microsoft.AspNetCore.Authorization
{
    /// <summary>
    /// 远程权限处理器
    /// </summary>
    public sealed class PermissionHandlerRemote : AbstractPermissionHandler
    {

        protected override Task<bool> CheckUserPermissions(Int64 userId, IEnumerable<string> codes, string validationVersion)
        {
            return Task.FromResult(true);
        }
    }
}
