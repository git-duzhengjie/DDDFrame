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
        protected readonly FrameDbContextBase FrameDbContext = frameDbContext;
        protected readonly EntityFactory EntityFactory = entityFactory;
        protected readonly IDomainServiceContext DomainServiceContext = domainServiceContext;

        public int InsertOrUpdatePriority => 0;

        public int RemovePriority => 0;

        public int QueryPriority => 0;

        public virtual async Task<FrameChangeOutputDTO> InsertOrUpdateAsync(params IInputDTO[] inputs)
        {
            var result = new FrameChangeOutputDTO();
            foreach (var input in inputs)
            {
                var iEntity = EntityFactory.GetEntity(input);
                if (iEntity is EntityBase entity)
                {
                    if (input.IsNew)
                    {
                        entity.Build();
                        entity.Build(DomainServiceContext);
                        DomainServiceContext.Add(entity,false);
                    }
                    else
                    {
                        var method = FrameDbContext.GetMethod("GetValueAsync");
                        method = method.MakeGenericMethod(entity.GetType());
                        var exist = await (method.Invoke(FrameDbContext, [entity.Id,false]) as Task<IEntity>) as EntityBase;
                        Debug.Assert(exist!=null);
                        exist.SetValue(entity);
                        exist.Build();
                        exist.Build(DomainServiceContext);
                        DomainServiceContext.Update(exist, false);
                    }
                    result.Changes.Add(entity.Output);
                }
            }
            return result;
        }

        public virtual async Task<IPagedList<IOutputDTO>> PageQueryAsync(IPageQueryDTO pageQueryDTO)
        {
            var entityType = EntityFactory.GetEntityType(pageQueryDTO.ObjectType);
            var method = FrameDbContext.GetMethod("PageQueryAsync");
            method=method.MakeGenericMethod(entityType);
            var ret = method.Invoke(FrameDbContext, [pageQueryDTO]);
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
                var entityType = EntityFactory.GetEntityType(queryDTO.ObjectType);
                var method = FrameDbContext.GetMethod("QueryAsync");
                method = method.MakeGenericMethod(entityType);
                var data = await (method.Invoke(FrameDbContext, [queryDTO]) as Task<IEntity[]>);
                result.Add([.. data.Select(x=>x.Output)]);
            }
            return [.. result];
        }

        public virtual async Task<int[]> CountAsync(params IQueryDTO[] queryDTOs)
        {
            var result = new List<int>();
            foreach (var queryDTO in queryDTOs)
            {
                var entityType = EntityFactory.GetEntityType(queryDTO.ObjectType);
                var method = FrameDbContext.GetMethod("CountAsync");
                method = method.MakeGenericMethod(entityType);
                var data = await (method.Invoke(FrameDbContext, [queryDTO]) as Task<int>);
                result.Add(data);
            }
            return [.. result];
        }

        public virtual async Task<FrameChangeOutputDTO> RemoveAsync(params IInputDTO[] removes)
        {
            var result = new FrameChangeOutputDTO();
            foreach (var remove in removes)
            {
                var entity = EntityFactory.GetEntity(remove) as EntityBase;
                DomainServiceContext.Remove(entity);
            }
            await Task.CompletedTask;
            return result;
        }
    }
}
