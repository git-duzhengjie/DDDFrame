﻿

namespace Microsoft.AspNetCore.Authorization
{
    /// <summary>
    /// 远程权限处理器
    /// </summary>
    public sealed class PermissionHandlerRemote : AbstractPermissionHandler
    {

        protected override Task<Boolean> CheckUserPermissions(Int64 userId, IEnumerable<String> codes, String validationVersion)
        {
            throw new NotImplementedException();
        }
    }
}
