
using System.ComponentModel;

namespace System
{
    public static class objectExtension
    {
        /// <summary>
        ///     A System.object extension method that toes the given this.
        /// </summary>
        /// <param name="this">this.</param>
        /// <param name="type">The type.</param>
        /// <returns>An object.</returns>
        public static object To(this object @this, Type type)
        {
            if (@this != null)
            {
                var targetType = type;

                if (@this.GetType() == targetType)
                {
                    return @this;
                }

                var converter = TypeDescriptor.GetConverter(@this);
                if (converter != null && converter.CanConvertTo(targetType))
                    return converter.ConvertTo(@this, targetType);

                converter = TypeDescriptor.GetConverter(targetType);
                if (converter != null && converter.CanConvertFrom(@this.GetType()))
                    return converter.ConvertFrom(@this);

                if (@this == DBNull.Value)
                    return null;

                return Convert.ChangeType(@this, targetType);
            }

            return @this;
        }

        /// <summary>
        ///     A System.object extension method that converts this object to an or default.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="this">this.</param>
        /// <param name="defaultValueFactory">The default value factory.</param>
        /// <returns>The given data converted to a T.</returns>
        public static T ToOrDefault<T>(this object @this, Func<object, T> defaultValueFactory)
        {
            try
            {
                return (T)@this.To(typeof(T));
            }
            catch (Exception)
            {
                return defaultValueFactory(@this);
            }
        }

        /// <summary>
        ///     A System.object extension method that converts this object to an or default.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="this">this.</param>
        /// <returns>The given data converted to a T.</returns>
        public static T ToOrDefault<T>(this object @this)
        {
            return @this.ToOrDefault(x => default(T));
        }

        /// <summary>
        ///     A T extension method that query if 'source' is the default value.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="source">The source to act on.</param>
        /// <returns>true if default, false if not.</returns>
        public static bool IsDefault<T>(this T source)
        {
            return typeof(T).IsValueType ? source.Equals(default(T)) : source == null;
        }

        /// <summary>
        ///     An object extension method that converts the @this to string or return an empty string if the value is null.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>@this as a string or empty if the value is null.</returns>
        public static string ToSafeString(this object @this)
        {
            return @this == null ? string.Empty : @this.ToString();
        }

        /// <summary>
        /// 将t2的值赋给t1
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        public static void SetValue<T>(this T t1,T t2)
        {
            if (t1 != null&&t2!=null)
            {
                var properties = typeof(T).GetProperties();
                foreach (var property in properties)
                {
                    if (property.CanWrite && property.CanRead)
                    {
                        var value = property.GetValue(t2);
                        property.SetValue(t1, value);
                    }
                }
            }
        }
    }
}
