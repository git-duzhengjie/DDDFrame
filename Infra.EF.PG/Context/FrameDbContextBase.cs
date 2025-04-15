using Infra.Core.Abstract;
using Infra.Core.Extensions;
using Infra.Core.Extensions.Entities;
using Infra.Core.Models;
using Infra.EF.Entities;
using Infra.EF.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Infra.EF.Context
{
    public abstract class FrameDbContextBase : DbContext, IFrameDbContext


    {
        protected readonly Assembly assembly;

        /// <summary>
        /// 是否是关系型数据库
        /// </summary>
        public abstract bool RelationDatabase { get; }

        public FrameDbContextBase(DbContextOptions dbContextOptions, IServiceInfo serviceInfo) : base(dbContextOptions)
        {
            assembly = serviceInfo?.GetDomainAssembly();
        }


        protected void IgnoreTypes(ModelBuilder modelBuilder)
        {
            if (assembly != null)
            {
                var ignoreTypes = assembly.GetExportedTypes()
                .Where(x => x.IsAssignableTo(typeof(EntityBase)))
                .Where(x => x.IsAbstract || x.GetCustomAttribute<NotMappedAttribute>() != null)
                .ToArray();
                ignoreTypes.ForEach(x => modelBuilder.Ignore(x));
            }
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IEntity> GetValueAsync<T>(long id, bool isTracking) where T : EntityBase

        {
            if (isTracking)
            {
                return await Set<T>().FirstOrDefaultAsync(x => x.Id == id);
            }
            else
            {
                return await Set<T>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            }
        }

        public async Task<IEntity[]> QueryAsync<T>(IQueryDTO query) where T : EntityBase
        {
            var querable = Set<T>().AsNoTracking().Where(query.GetExpressionFilter<T>());
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
            return await querable.ToArrayAsync();
        }

        public async Task<int> CountAsync<T>(IQueryDTO queryDTO) where T : EntityBase
        {
            var querable = Set<T>().AsNoTracking().Where(queryDTO.GetExpressionFilter<T>());
            return await querable.Select(x => x.Id).CountAsync();
        }

        public async Task<PagedList<IEntity>> PageQueryAsync<T>(IPageQueryDTO query) where T : EntityBase
        {
            int offset = (query.Page - 1) * query.Count;
            var querable = Set<T>()
                .AsNoTracking()
                .Where(query.GetExpressionFilter<T>())
                ;
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
            var data = await querable
                .Skip(offset)
                .Take(query.Count)
                .ToArrayAsync();
            decimal total = await querable.CountAsync();
            return new PagedList<IEntity>
            {
                DataList = data,
                Page = query.Page,
                Total = (int)total,
                Count = query.Count,
                Pages = (int)Math.Ceiling(total / query.Count),
            };
        }

        public async Task DeleteAsync<T>(params T[] entities) where T : EntityBase
        {
            var ids = entities.Select(x => x.Id).ToArray();
            var exists = await Set<T>().Where(x => ids.Contains(x.Id)).ToArrayAsync();
            if (exists.Length != 0)
            {
                RemoveRange(exists);
            }
        }

        public async Task UpdateAsync<T>(params T[] entities) where T : EntityBase
        {
            var ids = entities.Select(x => x.Id).ToArray();
            var exists = await Set<T>().Where(x => ids.Contains(x.Id)).ToArrayAsync();
            if (exists.Length != 0)
            {
                foreach (var entity in exists)
                {
                    var updateEntity = entities.FirstOrDefault(x => x.Id == entity.Id);
                    if (updateEntity != null)
                    {
                        entity.UpdatePropertyValue(updateEntity);
                    }
                }
            }
        }
        public async Task UpdateAsync<T>(params UpdateData[] entities) where T : EntityBase
        {
            var ids = entities.Select(x => x.Update.Id).ToArray();
            var exists = await Set<T>().Where(x => ids.Contains(x.Id)).ToArrayAsync();
            if (exists.Length != 0)
            {
                foreach (var entity in exists)
                {
                    var updateEntity = entities.FirstOrDefault(x => x.Update.Id == entity.Id);
                    if (updateEntity != null)
                    {
                        var properties = entity.GetType().GetProperties();
                        foreach (var property in properties)
                        {
                            if (updateEntity.Properties.Any(x => x == property.Name))
                            {
                                property.SetValue(entity, property.GetValue(updateEntity.Update));
                            }
                        }
                    }
                }
            }
        }

        public async Task AddAsync<T>(params T[] entities) where T : EntityBase
        {
            if (entities.Length != 0)
            {
                await AddRangeAsync(entities);
            }
        }
    }
}
