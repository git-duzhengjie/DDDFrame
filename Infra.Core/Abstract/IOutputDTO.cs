using UniversalRpc.Abstracts;

namespace Infra.Core.Abstract
{
    public interface IOutputDTO:IObject
    {
        /// <summary>
        /// 输入对象
        /// </summary>
        public IInputDTO GetInput();

        /// <summary>
        /// 唯一id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 最后一次时间
        /// </summary>
        public DateTime LastTime { get; set; }
    }
}
