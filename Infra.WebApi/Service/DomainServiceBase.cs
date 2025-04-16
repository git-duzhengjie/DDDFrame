using Infra.Core.Abstract;
using Infra.Core.DTOs;
using Infra.Core.Models;
using Infra.EF.Context;
using Infra.EF.Entities;
using Infra.EF.Service;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;

namespace Infra.WebApi.Service
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="frameDbContext"></param>
    public abstract class DomainServiceBase(FrameDbContextBase frameDbContext,
        EntityFactory entityFactory,
        IDomainServiceContext domainServiceContext) : IDomainService
    {
        protected readonly FrameDbContextBase frameDbContext = frameDbContext;
        protected readonly EntityFactory entityFactory = entityFactory;
        protected readonly IDomainServiceContext domainServiceContext = domainServiceContext;

        public int InsertOrUpdatePriority => 0;

        public int RemovePriority => 0;

        public int QueryPriority => 0;

        public virtual async Task<FrameChangeOutputDTO> InsertOrUpdateAsync(params IInputDTO[] inputs)
        {
            var result = new FrameChangeOutputDTO();
            foreach (var input in inputs)
            {
                var iEntity = entityFactory.GetEntity(input);
                if (iEntity is EntityBase entity)
                {
                    if (input.IsNew)
                    {
                        entity.Build();
                        entity.Build(domainServiceContext);
                        domainServiceContext.Add(entity,false);
                    }
                    else
                    {
                        var method = frameDbContext.GetMethod("GetValueAsync");
                        method = method.MakeGenericMethod(entity.GetType());
                        var exist = await (method.Invoke(frameDbContext, [entity.Id,false]) as Task<IEntity>) as EntityBase;
                        Debug.Assert(exist!=null);
                        exist.SetValue(entity);
                        exist.Build();
                        exist.Build(domainServiceContext);
                        domainServiceContext.Update(exist, false);
                    }
                    result.Changes.Add(entity.Output);
                }
            }
            return result;
        }

        public virtual async Task<IPagedList<IOutputDTO>> PageQueryAsync(IPageQueryDTO pageQueryDTO)
        {
            var entityType = entityFactory.GetEntityType(pageQueryDTO.ObjectType);
            var method = frameDbContext.GetMethod("PageQueryAsync");
            method=method.MakeGenericMethod(entityType);
            var ret = method.Invoke(frameDbContext, [pageQueryDTO]);
            var data = await (ret as Task<PagedList<IEntity>>);
            return new PagedList<IOutputDTO>
            {
                DataList = [.. data.DataList.OfType<EntityBase>().Select(x => x.Output)],
                Page = data.Page,
                Count = data.Count,
                Total = data.Total,
                Pages = data.Pages,
            };
        }

        public virtual async Task<IEnumerable<IOutputDTO[]>> QueryAsync(params IQueryDTO[] queryDTOs)
        {
            var result=new List<IOutputDTO[]>();
            foreach(var queryDTO in queryDTOs)
            {
                var entityType = entityFactory.GetEntityType(queryDTO.ObjectType);
                var method = frameDbContext.GetMethod("QueryAsync");
                method = method.MakeGenericMethod(entityType);
                var data = await (method.Invoke(frameDbContext, [queryDTO]) as Task<IEntity[]>);
                result.Add([.. data.Select(x=>x.Output)]);
            }
            return [.. result];
        }

        public virtual async Task<int[]> CountAsync(params IQueryDTO[] queryDTOs)
        {
            var result = new List<int>();
            foreach (var queryDTO in queryDTOs)
            {
                var entityType = entityFactory.GetEntityType(queryDTO.ObjectType);
                var method = frameDbContext.GetMethod("CountAsync");
                method = method.MakeGenericMethod(entityType);
                var data = await (method.Invoke(frameDbContext, [queryDTO]) as Task<int>);
                result.Add(data);
            }
            return [.. result];
        }

        public virtual async Task<FrameChangeOutputDTO> RemoveAsync(params IInputDTO[] removes)
        {
            var result = new FrameChangeOutputDTO();
            foreach (var remove in removes)
            {
                var entity = entityFactory.GetEntity(remove) as EntityBase;
                domainServiceContext.Remove(entity);
            }
            await Task.CompletedTask;
            return result;
        }
    }
}
