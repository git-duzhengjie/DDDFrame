using Infra.Core.Abstract;
using Infra.Core.DTO;

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
        /// 添加或者更新对象处理优先级
        /// </summary>
        public int InsertOrUpdatePriority { get; }

        /// <summary>
        /// 删除对象处理优先级
        /// </summary>
        public int RemovePriority { get; }
    }
}
