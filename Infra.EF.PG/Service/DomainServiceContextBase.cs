using Infra.EF.Context;
using Infra.EF.Entities;
using Microsoft.Extensions.Logging;

namespace Infra.EF.Service
{
    public abstract class DomainServiceContextBase(
        FrameDbContextBase frameDbContext, 
        IDbData dbData,
        ILoggerFactory loggerFactory) 
        : IDomainServiceContext
    {
        protected readonly FrameDbContextBase frameDbContext = frameDbContext;
        protected readonly IDbData dbData = dbData;
        protected readonly ILogger logger = loggerFactory.CreateLogger("DomainServiceContextBase");
        protected readonly List<(EntityBase,bool)> adds=[];
        protected readonly List<EntityBase> removes=[];
        protected readonly List<(EntityBase, bool)> updates=[];
        protected readonly List<UpdateData> updateProperties=[];

        public virtual void Add(EntityBase entity,bool withNavigation)
        {
            if (adds.Any(a => a.Item1.Id == entity.Id))
            {
                adds.RemoveWhere(a=>a.Item1.Id==entity.Id);
            }
            adds.Add((entity,withNavigation));
        }

        public virtual void Remove(EntityBase entity)
        {
            if (adds.Any(a => a.Item1.Id == entity.Id))
            {
                adds.RemoveWhere(a=>a.Item1.Id==entity.Id);
            }
            else
            {
                updates.RemoveWhere(r=>r.Item1.Id==entity.Id);
                removes.Add(entity);
            }
        }

        public virtual async Task SaveAsync()
        {
            if (!frameDbContext.RelationDatabase)
            {
                frameDbContext.Database.AutoTransactionBehavior = Microsoft.EntityFrameworkCore.AutoTransactionBehavior.Never;
            }
            var transaction = frameDbContext.RelationDatabase? await frameDbContext.Database.BeginTransactionAsync():null;
            try
            {
                if (removes.Count != 0)
                {
                    await dbData.DeleteAsync([.. removes]);
                    removes.Clear();
                }
                if (adds.Count != 0)
                {
                    var group = adds.GroupBy(x => x.Item2).OrderBy(x => x.Key);
                    foreach (var g in group)
                    {
                        var adds = g.Select(x => x.Item1).ToArray();
                        await dbData.AddAsync(g.Key, adds);
                    }
                    adds.Clear();
                }
                if (updates.Count != 0)
                {
                    var group = updates.GroupBy(x => x.Item2).OrderBy(x => x.Key);
                    foreach (var g in group)
                    {
                        var updates = g.Select(x => x.Item1).ToArray();
                        await dbData.UpdateAsync(g.Key, updates);
                    }
                    updates.Clear();
                }
                if (updateProperties.Count != 0)
                {
                    await dbData.UpdateAsync([.. updateProperties]);
                    updateProperties.Clear();
                }
                
                if (transaction == null)
                {
                    await frameDbContext.SaveChangesAsync();
                }
                else
                {
                    await transaction?.CommitAsync();
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex,"保存数据异常");
                if (transaction != null)
                {
                    await transaction?.RollbackAsync();
                }
                throw;
            }
            
        }

        public virtual void Update(EntityBase entity,bool withNavigation)
        {
            if (!removes.Any(r => entity.Id == r.Id))
            {
                updates.Add((entity,withNavigation));
            }
        }

        public virtual void Update(EntityBase entity, string[] propertyNames)
        {
            if (!removes.Any(r => entity.Id == r.Id))
            {
                updateProperties.Add(new UpdateData
                {
                    Update=entity,
                    Properties=propertyNames
                });
            }
        }
    }
}
