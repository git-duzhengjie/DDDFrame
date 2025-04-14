
using System;

namespace Infra.WebApi.Consts.Permissions.User
{
    /// <summary>
    /// 权限常量
    /// </summary>
    public static class PermissionConsts
    {
        public static class User
        {
            public const String Create = "userAdd";
            public const String Update = "userEdit";
            public const String Delete = "userDelete";
            public const String SetRole = "userSetRole";
            public const String GetList = "userList";
            public const String ChangeStatus = "userFreeze";
        }

        public static class Dept
        {
            public const String Create = "deptAdd";
            public const String Update = "deptEdit";
            public const String Delete = "deptDelete";
            public const String GetList = "deptList";
        }

        public static class Menu
        {
            public const String Create = "menuAdd";
            public const String Update = "menuEdit";
            public const String Delete = "menuDelete";
            public const String GetList = "menuList";
        }

        public static class Role
        {
            public const String Create = "roleAdd";
            public const String Update = "roleEdit";
            public const String Delete = "roleDelete";
            public const String GetList = "roleList";
            public const String SetPermissons = "roleSetAuthority";
        }
    }
}
