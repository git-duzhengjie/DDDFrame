using Infra.EF.PG.Context;
using Infra.EF.PG.Entities;

namespace Infra.EF.PG.Service
{
    public abstract class DomainServiceContextBase : IDomainServiceContext
    {
        private FrameDbContext frameDbContext;
        private IDbData dbData;
        private List<(EntityBase,bool)> adds=new List<(EntityBase, bool)>();
        private List<EntityBase> removes=new List<EntityBase>();
        private List<(EntityBase, bool)> updates=new List<(EntityBase, bool)>();
        private List<UpdateData> updateProperties=new List<UpdateData>();

        public DomainServiceContextBase(FrameDbContext frameDbContext,IDbData dbData) { 
            this.frameDbContext=frameDbContext;
            this.dbData=dbData;
        }
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
            var transaction = await frameDbContext.Database.BeginTransactionAsync();
            try
            {
                if (removes.Count != 0)
                {
                    await dbData.DeleteAsync(removes.ToArray());
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
                    await dbData.UpdateAsync(updateProperties.ToArray());
                    updateProperties.Clear();
                }
                await transaction.CommitAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                await transaction.RollbackAsync();
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
