using Infra.Core.Abstract;
using Infra.Core.DTOs;
using Infra.WebApi.DTOs;

namespace Infra.WebApi.Service
{
    /// <summary>
    /// 微服务主服务接口
    /// </summary>
    public interface IMainAppService:IAppService
    {
        /// <summary>
        /// 服务的增删改
        /// </summary>
        /// <param name="frameChangeInput"></param>
        /// <returns></returns>
        Task<FrameChangeOutputDTO> CommitAsync(FrameChangeInputDTO frameChangeInput);

        /// <summary>
        /// 服务的查询
        /// </summary>
        /// <param name="queryDTO"></param>
        /// <returns></returns>
        Task<IEnumerable<IOutputDTO[]>> QueryAsync(params IQueryDTO[] queryDTOs);

        /// <summary>
        /// 服务的统计查询
        /// </summary>
        /// <param name="queryDTO"></param>
        /// <returns></returns>
        Task<int[]> CountAsync(params IQueryDTO[] queryDTOs);

        /// <summary>
        /// 服务的分页查询
        /// </summary>
        /// <param name="pageQueryDTO"></param>
        /// <returns></returns>
        Task<IPagedList<IOutputDTO>> PageQueryAsync(IPageQueryDTO pageQueryDTO);
        
        /// <summary>
        /// 根据枚举名查询枚举值
        /// </summary>
        /// <param name="enumName"></param>
        /// <returns></returns>
        EnumDTO[] GetEnums(string enumName);
    }
}
