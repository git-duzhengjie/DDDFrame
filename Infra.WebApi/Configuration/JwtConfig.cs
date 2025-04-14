
using System;

namespace Infra.WebApi.Configuration
{
    /// <summary>
    /// Jwt配置
    /// </summary>
    public class JwtConfig
    {
        /// <summary>
        /// 加密Key
        /// </summary>
        public string SymmetricSecurityKey { get; set; }

        /// <summary>
        /// 颁发者
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// 时间歪斜，单位秒
        /// </summary>
        public Int32 ClockSkew { get; set; }

        /// <summary>
        /// Accessoken受众
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// RefreshToken受众
        /// </summary>
        public string RefreshTokenAudience { get; set; }

        /// <summary>
        /// AccessToken过期时间，单位分钟
        /// </summary>
        public Int32 Expire { get; set; }

        /// <summary>
        /// RefreshToken过期时间，单位分钟
        /// </summary>
        public Int32 RefreshTokenExpire { get; set; }
    }
}
