using Infra.Core.Abstract;
using Infra.Core.Models;
using Infra.EF.Entities;
using Infra.EF.Service;

namespace Infra.EF.Context
{
    public interface IFrameDbContext
    {
        /// <summary>
        /// 获取实体值
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="isTracking"></param>
        /// <returns></returns>
        Task<IEntity> GetValueAsync<T>(long id, bool isTracking) where T : EntityBase;

        /// <summary>
        /// 查询实体
        /// </summary>
        /// <param name="queryDTO"></param>
        /// <param name="queryType"></param>
        /// <returns></returns>
        Task<IEntity[]> QueryAsync<T>(IQueryDTO queryDTO) where T : EntityBase;

        /// <summary>
        /// 统计实体个数
        /// </summary>
        /// <param name="queryDTO"></param>
        /// <param name="queryType"></param>
        /// <returns></returns>
        Task<int> CountAsync<T>(IQueryDTO queryDTO) where T : EntityBase;

        /// <summary>
        /// 分页查询实体
        /// </summary>
        /// <param name="queryDTO"></param>
        /// <param name="queryType"></param>
        /// <returns></returns>
        Task<PagedList<IEntity>> PageQueryAsync<T>(IPageQueryDTO queryDTO) where T : EntityBase;

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task DeleteAsync<T>(params T[] entities) where T : EntityBase;

        /// <summary>
        /// 修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task UpdateAsync<T>(params T[] entities) where T : EntityBase;

        /// <summary>
        /// 局部修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task UpdateAsync<T>(params UpdateData[] entities) where T : EntityBase;

        /// <summary>
        /// 添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task AddAsync<T>(params T[] entities) where T : EntityBase;
    }
}