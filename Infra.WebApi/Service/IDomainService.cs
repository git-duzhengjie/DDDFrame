using Infra.Core.Abstract;
using Infra.Core.DTOs;

namespace Infra.WebApi.Service
{
    public interface IDomainService
    {
        /// <summary>
        /// 添加或者更新对象
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public Task<FrameChangeOutputDTO> InsertOrUpdateAsync(params IInputDTO[] inputs);

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="removes"></param>
        /// <returns></returns>
        public Task<FrameChangeOutputDTO> RemoveAsync(params IInputDTO[] removes);

        /// <summary>
        /// 查询对象
        /// </summary>
        /// <param name="queryDTOs"></param>
        /// <returns></returns>
        public Task<IEnumerable<IOutputDTO[]>> QueryAsync(params IQueryDTO[] queryDTOs);

        /// <summary>
        /// 统计对象
        /// </summary>
        /// <param name="queryDTOs"></param>
        /// <returns></returns>
        public Task<int[]> CountAsync(params IQueryDTO[] queryDTOs);

        /// <summary>
        /// 分页查询对象
        /// </summary>
        /// <param name="pageQueryDTOs"></param>
        /// <returns></returns>
        public Task<IPagedList<IOutputDTO>> PageQueryAsync(IPageQueryDTO pageQueryDTO);

        /// <summary>
        /// 添加或者更新对象处理优先级
        /// </summary>
        public int InsertOrUpdatePriority { get; }

        /// <summary>
        /// 删除对象处理优先级
        /// </summary>
        public int RemovePriority { get; }

        /// <summary>
        /// 服务的查询优先级
        /// </summary>
        public int QueryPriority { get; }
    }
}
