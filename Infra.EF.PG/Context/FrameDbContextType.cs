using Infra.Core.Abstract;
using Infra.Core.Extensions.Entities;
using Infra.Core.Models;
using Infra.EF.PG.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infra.EF.PG.Context
{
    public class FrameDbContextType<T> where T : EntityBase
    {
        public static T GetValue(int id, FrameDbContext frameDbContext, bool isTracking = false)
        {
            if (isTracking)
            {
                return frameDbContext.Set<T>().FirstOrDefault(x => x.Id == id);
            }
            else
            {
                return frameDbContext.Set<T>().AsNoTracking().FirstOrDefault(x => x.Id == id);
            }
        }

        public static Task<T[]> QueryAsync(IQueryDTO queryDTO, FrameDbContext frameDbContext)
        {
            return frameDbContext.Set<T>().AsNoTracking().Where(queryDTO.GetExpressionFilter<T>()).ToArrayAsync();
        }

        public static async Task<IPagedList<T>> PageQueryAsync(IPageQueryDTO query, FrameDbContext frameDbContext)
        {
            int offset = (int)((query.Page - 1) * query.Count);
            var querable = frameDbContext.Set<T>()
                .AsNoTracking()
                .Where(query.GetExpressionFilter<T>());
            if (query.Order.IsNotNullOrEmpty())
            {
                if (query.OrderDesc)
                {
                    querable = querable.OrderByDescending(query.Order);
                }
                else
                {
                    querable = querable.OrderBy(query.Order);
                }
            }
            else
            {
                querable = querable.OrderByDescending(typeof(T).Key());
            }
            var data = await querable.Skip(offset).Take(query.Count).ToArrayAsync();
            var total = await querable.CountAsync();
            return new PagedList<T>
            {
                DataList = data,
                Page = query.Page,
                Total = total,
                Count = query.Count,
                Pages = total / query.Count,
            };
        }
    }
}
