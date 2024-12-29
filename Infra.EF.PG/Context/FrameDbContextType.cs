using Infra.Core.Abstract;
using Infra.Core.Extensions.Entities;
using Infra.Core.Models;
using Infra.EF.PG.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infra.EF.PG.Context
{
    public class FrameDbContextType<T> where T : EntityBase
    {
        public static T GetValue(long id, FrameDbContext frameDbContext, bool isTracking)
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

        public static object[] Query(IQueryDTO queryDTO, FrameDbContext frameDbContext)
        {
            return frameDbContext.Set<T>().AsNoTracking().Where(queryDTO.GetExpressionFilter<T>()).ToArray();
        }

        public static IPagedList<object> PageQuery(IPageQueryDTO query, FrameDbContext frameDbContext)
        {
            int offset = (query.Page - 1) * query.Count;
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
            var data = querable.Skip(offset).Take(query.Count).ToArray();
            var total = querable.Count();
            return new PagedList<object>
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
