
namespace System.Linq.Expressions
{
    public static class ExpressionCreator
    {
        public static Expression<Func<T, Boolean>> New<T>(Expression<Func<T, Boolean>>? expr = null)
            => expr ?? (x => true);

        public static Expression<Func<T1, T2, Boolean>> New<T1, T2>(Expression<Func<T1, T2, Boolean>>? expr = null)
            => expr ?? ((x, y) => true);

        public static Expression<Func<T1, T2, T3, Boolean>> New<T1, T2, T3>(Expression<Func<T1, T2, T3, Boolean>>? expr = null)
            => expr ?? ((x, y, z) => true);
    }
}
