using Infra.EF.PG.Entities;

namespace Infra.EF.PG.Service
{
    public interface IDomainServiceContext
    {
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity"></param>
        public void Add(EntityBase entity,bool withNavigation);

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        public void Remove(EntityBase entity);

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity"></param>
        public void Update(EntityBase entity,bool withNavigation);

        /// <summary>
        /// 更新实体指定熟悉
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propertyNames"></param>
        void Update(EntityBase entity, string[] propertyNames);

        /// <summary>
        /// 存储
        /// </summary>
        /// <returns></returns>
        public Task SaveAsync();
    }
}
