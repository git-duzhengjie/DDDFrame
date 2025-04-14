
using System;

namespace Infra.WebApi.Consts.Caching.User
{
    public class CachingConsts : Common.CachingConsts
    {
        //cache key
        public const String MenuListCacheKey = "user:menus:list";

        public const String MenuTreeListCacheKey = "user:menus:tree";
        public const String MenuRelationCacheKey = "user:menus:relation";
        public const string MenuCodesCacheKey = "user:menus:codes";

        public const String DetpListCacheKey = "user:depts:list";
        public const String DetpTreeListCacheKey = "user:depts:tree";
        public const String DetpSimpleTreeListCacheKey = "user:depts:tree:simple";

        public const String RoleListCacheKey = "user:roles:list";

        //cache prefix
        public const String UserValidateInfoKeyPrefix = "user:users:validateinfo";
    }
}
