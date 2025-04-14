using Infra.Core.Abstract;
using Infra.Core.Extensions.Entities;
using Infra.Core.Models;
using Infra.EF.Entities;
using Infra.EF.Service;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Infra.EF.Context
{
    public class FrameDbContextType<T> where T : EntityBase
    {
        public static T GetValue(long id, FrameDbContextBase frameDbContext, bool isTracking)
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

        public static T[] Query(IQueryDTO query, FrameDbContextBase frameDbContext)
        {
            var querable = frameDbContext.Set<T>().AsNoTracking().Where(query.GetExpressionFilter<T>());
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
            return querable.ToArray();
        }

        public static int Count(IQueryDTO query, FrameDbContextBase frameDbContext)
        {
            var querable = frameDbContext.Set<T>().AsNoTracking().Where(query.GetExpressionFilter<T>());
            return querable.Select(x => x.Id).Count();
        }

        public static PagedList<T> PageQuery(IPageQueryDTO query, FrameDbContextBase frameDbContext)
        {
            try
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
                decimal total = querable.Count();
                return new PagedList<T>
                {
                    DataList = data,
                    Page = query.Page,
                    Total = (int)total,
                    Count = query.Count,
                    Pages = (int)Math.Ceiling(total / query.Count),
                };
            }
            catch (Exception ex) {
                return null;
            }
        }

        public static void Delete(FrameDbContextBase frameDbContext, params T[] entities)
        {
            var ids = entities.Select(x => x.Id).ToArray();
            var exists = frameDbContext.Set<T>().Where(x => ids.Contains(x.Id)).ToArray();
            if (exists.Length != 0)
            {
                frameDbContext.Set<T>().RemoveRange(exists);
            }
        }

        public static void Update(FrameDbContextBase frameDbContext, params T[] entities)
        {
            var ids = entities.Select(x => x.Id).ToArray();
            var exists = frameDbContext.Set<T>().Where(x => ids.Contains(x.Id)).ToArray();
            if (exists.Length != 0)
            {
                foreach (var entity in exists)
                {
                    var updateEntity = entities.FirstOrDefault(x => x.Id == entity.Id);
                    if (updateEntity != null)
                    {
                        frameDbContext.Entry(entity).CurrentValues.SetValues(updateEntity);
                    }
                }
            }
        }
        public static void Update(FrameDbContextBase frameDbContext, params UpdateData[] entities)
        {
            var ids = entities.Select(x => x.Update.Id).ToArray();
            var exists = frameDbContext.Set<T>().Where(x => ids.Contains(x.Id)).ToArray();
            if (exists.Length != 0)
            {
                foreach (var entity in exists)
                {
                    var updateEntity = entities.FirstOrDefault(x => x.Update.Id == entity.Id);
                    if (updateEntity != null)
                    {
                        var properties = entity.GetType().GetProperties();
                        foreach(var property in properties)
                        {
                            if (updateEntity.Properties.Any(x => x == property.Name))
                            {
                                property.SetValue(updateEntity.Update.GetPropertyValue(property.Name));
                            }
                        }
                    }
                }
            }
        }

        public static void Add(FrameDbContextBase frameDbContext, params T[] entities)
        {
            if (entities.Length!=0)
            {
                frameDbContext.Set<T>().AddRange(entities);
            }
        }
    }
}
