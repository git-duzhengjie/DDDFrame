
using System;

namespace Infra.Cache.Core
{
    /// <summary>
    /// 缓存值
    /// </summary>
    public class CacheValue<T>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="hasValue">是否有值</param>
        public CacheValue(T value, bool hasValue)
        {
            Value = value;
            HasValue = hasValue;
        }

        /// <summary>
        /// 是否有值
        /// </summary>
        public bool HasValue { get; }

        /// <summary>
        /// 值是否为空
        /// </summary>
        public bool IsNull => Value == null;

        /// <summary>
        /// 值
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// 空值
        /// </summary>
        public static CacheValue<T> Null { get; } = new CacheValue<T>(default(T), true);

        /// <summary>
        /// 没有值
        /// </summary>
        public static CacheValue<T> NoValue { get; } = new CacheValue<T>(default(T), false);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value?.ToString() ?? "<null>";
        }
    }
}
