
namespace Infra.EF.PG.Internal
{
    internal static class EFCoreUtil
    {
        internal static Object[] GetEntityKeyValues<TEntity>(Func<TEntity, Object>[] keyValueGetter, TEntity entity)
            => keyValueGetter.Select(x => x.Invoke(entity)).ToArray();
    }
}
