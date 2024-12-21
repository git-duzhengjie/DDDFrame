using Infra.Core.Abstract;
using Infra.Core.DTO;
using Infra.EF.PG;
using Infra.EF.PG.Entities;
using Infra.EF.PG.Service;

namespace Infra.WebApi.Service
{
    public abstract class DomainServiceBase:IDomainService
    {
        private readonly FrameDbContext _frameDbContext;
        private readonly EntityFactory _entityFactory;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="frameDbContext"></param>
        public DomainServiceBase(FrameDbContext frameDbContext, EntityFactory entityFactory)
        {
            _frameDbContext = frameDbContext;
            _entityFactory = entityFactory;
        }
        public int InsertOrUpdatePriority => 0;

        public int RemovePriority => 0;

        public virtual async Task<FrameChangeOutputDTO> InsertOrUpdateAsync(params IInputDTO[] inputs)
        {
            var result = new FrameChangeOutputDTO();
            foreach (var input in inputs)
            {
                var iEntity = _entityFactory.GetEntity(input);
                if (iEntity is EntityBase entity)
                {
                    entity.Build();
                    if (input.IsNew)
                    {
                        await _frameDbContext.AddAsync(entity);
                    }
                    else
                    {
                        var exist = await _frameDbContext.GetValueAsync(input.Id, entity.GetType(), true);
                        exist.SetValue(entity);
                    }
                    result.Changes.Add(_entityFactory.GetOutput(entity));
                }
            }
            await _frameDbContext.SaveChangesAsync();
            return result;
        }

        public virtual async Task<FrameChangeOutputDTO> RemoveAsync(params IInputDTO[] removes)
        {
            var result = new FrameChangeOutputDTO();
            foreach (var remove in removes)
            {
                var entity = _entityFactory.GetEntity(remove);
                var exist = await _frameDbContext.GetValueAsync(remove.Id, entity.GetType(), true);
                _frameDbContext.Remove(exist);
                result.Deletes.Add(_entityFactory.GetOutput(entity));
            }
            await _frameDbContext.SaveChangesAsync();
            return result;
        }
    }
}
