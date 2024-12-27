using Infra.Core.Abstract;
using Infra.Core.DTO;
using Infra.Core.Models;
using Infra.EF.PG.Context;
using Infra.EF.PG.Entities;
using Infra.EF.PG.Service;
using Infra.WebApi.Models;

namespace Infra.WebApi.Service
{
    public abstract class DomainServiceBase:IDomainService
    {
        private readonly FrameDbContext frameDbContext;
        private readonly EntityFactory entityFactory;
        private readonly IDomainServiceContext domainServiceContext;
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
            this.frameDbContext = frameDbContext;
            this.entityFactory = entityFactory;
            this.domainServiceContext = domainServiceContext;
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
                var iEntity = entityFactory.GetEntity(input);
                if (iEntity is EntityBase entity)
                {
                    entity.Build();
                    entity.Build(domainServiceContext);
                    if (input.IsNew)
                    {
                        domainServiceContext.Add(entity,false);
                    }
                    else
                    {
                        var exist = await frameDbContext.GetValueAsync(input.Id, entity.GetType(), true) as EntityBase;
                        if (exist != null)
                        {
                            exist.SetValue(entity);
                            domainServiceContext.Update(exist, false);
                        }
                    }
                    result.Changes.Add(entity.Output);
                }
            }
            return result;
        }

        public virtual async Task<IPagedList<IOutputDTO>> PageQueryAsync(IPageQueryDTO pageQueryDTO)
        {
            var entityType = entityFactory.GetEntityType(pageQueryDTO.ObjectType);
            var data = await frameDbContext.PageQueryAsync(pageQueryDTO, entityType);
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
                var entityType = entityFactory.GetEntityType(queryDTO.ObjectType);
                var data = (await frameDbContext.QueryAsync(queryDTO, entityType)).OfType<IEntity>();
                result.AddRange(data.Select(x=>x.Output));
            }
            return [.. result];
        }

        public virtual async Task<FrameChangeOutputDTO> RemoveAsync(params IInputDTO[] removes)
        {
            var result = new FrameChangeOutputDTO();
            foreach (var remove in removes)
            {
                var entity = entityFactory.GetEntity(remove);
                var exist = await frameDbContext.GetValueAsync(remove.Id, entity.GetType(), true) as EntityBase;
                domainServiceContext.Remove(exist);
                result.Deletes.Add(entity.Output);
            }
            return result;
        }
    }
}
