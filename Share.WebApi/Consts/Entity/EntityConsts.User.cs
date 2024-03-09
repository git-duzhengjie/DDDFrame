
using System;

namespace Infra.WebApi.Consts.Entity
{
    /// <summary>
    /// 用户常量
    /// </summary>
    public static class UserConsts
    {
        public const Int32 Account_MaxLength = 16;
        //public const Int32 Avatar_MaxLength = 64;
        public const Int32 Email_Maxlength = 32;
        public const Int32 Name_Maxlength = 16;
        public const Int32 Password_Maxlength = 32;
        public const Int32 Phone_Maxlength = 11;
        //public const Int32 RoleIds_Maxlength = 72;
        //public const Int32 Salt_Maxlength = 6;
    }

    public static class RoleConsts
    {
        public const Int32 Name_MaxLength = 32;
        public const Int32 Tips_MaxLength = 64;
    }

    public static class DeptConsts
    {
        public const int FullName_MaxLength = 32;
        public const int SimpleName_MaxLength = 16;
        public const int Tips_MaxLength = 64;
        public const int Pids_MaxLength = 80;
    }

    public static class MenuConsts
    {
        public const Int32 Code_MaxLength = 32;
        public const Int32 PCode_MaxLength = 32;
        public const Int32 PCodes_MaxLength = 256;
        public const Int32 Component_MaxLength = 64;
        public const Int32 Icon_MaxLength = 32;
        public const Int32 Name_MaxLength = 16;
        public const Int32 Tips_MaxLength = 32;
        public const Int32 Url_MaxLength = 64;
    }
}
