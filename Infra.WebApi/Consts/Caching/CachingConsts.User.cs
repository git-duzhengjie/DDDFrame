
using System;

namespace Infra.WebApi.Consts.Caching.User
{
    public class CachingConsts : Common.CachingConsts
    {
        //cache key
        public const string MenuListCacheKey = "user:menus:list";

        public const string MenuTreeListCacheKey = "user:menus:tree";
        public const string MenuRelationCacheKey = "user:menus:relation";
        public const string MenuCodesCacheKey = "user:menus:codes";

        public const string DetpListCacheKey = "user:depts:list";
        public const string DetpTreeListCacheKey = "user:depts:tree";
        public const string DetpSimpleTreeListCacheKey = "user:depts:tree:simple";

        public const string RoleListCacheKey = "user:roles:list";

        //cache prefix
        public const string UserValidateInfoKeyPrefix = "user:users:validateinfo";
    }
}
