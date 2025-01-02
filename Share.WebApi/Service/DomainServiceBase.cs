using Infra.Core.Abstract;
using Infra.Core.DTO;
using Infra.Core.Models;
using Infra.EF.PG.Context;
using Infra.EF.PG.Entities;
using Infra.EF.PG.Service;
using Infra.WebApi.Models;
using System.Diagnostics;

namespace Infra.WebApi.Service
{
    public abstract class DomainServiceBase:IDomainService
    {
        protected readonly FrameDbContext FrameDbContext;
        protected readonly EntityFactory EntityFactory;
        protected readonly IDomainServiceContext DomainServiceContext;
        protected readonly LoginUser LoginUser;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frameDbContext"></param>
        public DomainServiceBase(FrameDbContext frameDbContext, 
            EntityFactory entityFactory,
            IDomainServiceContext domainServiceContext,
            LoginUser loginUser)
        {
            FrameDbContext = frameDbContext;
            EntityFactory = entityFactory;
            DomainServiceContext = domainServiceContext;
            LoginUser = loginUser;
        }
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
                        var exist = await FrameDbContext.GetValueAsync(input.Id, entity.GetType(), false) as EntityBase;
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
            var data = await FrameDbContext.PageQueryAsync(pageQueryDTO, entityType);
            return new PagedList<IOutputDTO>
            {
                DataList = data.DataList.OfType<IEntity>().Select(x => x.Output).ToList(),
                Page = data.Page,
                Count = data.Count,
                Total = data.Total,
                Pages = data.Pages,
            };
        }

        public virtual async Task<IOutputDTO[]> QueryAsync(params IQueryDTO[] queryDTOs)
        {
            var result=new List<IOutputDTO>();
            foreach(var queryDTO in queryDTOs)
            {
                var entityType = EntityFactory.GetEntityType(queryDTO.ObjectType);
                var data = (await FrameDbContext.QueryAsync(queryDTO, entityType)).OfType<IEntity>();
                result.AddRange(data.Select(x=>x.Output));
            }
            return [.. result];
        }

        public virtual async Task<FrameChangeOutputDTO> RemoveAsync(params IInputDTO[] removes)
        {
            var result = new FrameChangeOutputDTO();
            foreach (var remove in removes)
            {
                var entity = EntityFactory.GetEntity(remove);
                var exist = await FrameDbContext.GetValueAsync(remove.Id, entity.GetType(), true) as EntityBase;
                DomainServiceContext.Remove(exist);
                //result.Deletes.Add(exist.Output);
            }
            return result;
        }
    }
}
