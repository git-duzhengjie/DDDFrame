using System;
using System.Threading.Tasks;

namespace Infra.Cache
{
    /// <summary>
    /// 分布式锁接口
    /// </summary>
    public interface IDistributedLocker
    {
        /// <summary>
        /// 获取分布式锁
        /// </summary>
        /// <param name="cacheKey">cacheKey.</param>
        /// <param name="timeoutSeconds">锁定时间</param>
        /// <param name="autoDelay">是否自己续期</param>
        /// <returns>Success 获取锁的状态，LockValue锁的版本号</returns>
        Task<(Boolean Success, String LockValue)> LockAsync(String cacheKey, Int32 timeoutSeconds = 5, Boolean autoDelay = false);

        /// <summary>
        /// 安全解锁
        /// </summary>
        /// <param name="cacheKey">cacheKey.</param>
        /// <param name="cacheValue">版本号</param>
        /// <returns></returns>
        Task<Boolean> SafedUnLockAsync(String cacheKey, String cacheValue);

        /// <summary>
        /// 获取分布式锁
        /// </summary>
        /// <param name="cacheKey">cacheKey.</param>
        /// <param name="timeoutSeconds">锁定时间</param>
        /// <param name="autoDelay">是否自己续期</param>
        /// <returns>Success 获取锁的状态，LockValue锁的版本号</returns>
        (Boolean Success, String LockValue) Lock(String cacheKey, Int32 timeoutSeconds = 5, Boolean autoDelay = false);

        /// <summary>
        /// 安全解锁
        /// </summary>
        /// <param name="cacheKey">cacheKey.</param>
        /// <param name="cacheValue">版本号</param>
        /// <returns></returns>
        Boolean SafedUnLock(String cacheKey, String cacheValue);
    }
}
