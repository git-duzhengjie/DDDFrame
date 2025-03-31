using UniversalRpc.Abstracts;

namespace Infra.Core.Abstract
{
    public interface IQueryDTO:IObject
    {
        /// <summary>
        /// 排序字段
        /// </summary>
        public string Order { get; set; }

        /// <summary>
        /// 是否降序
        /// </summary>
        public bool OrderDesc { get; set; }

        /// <summary>
        /// 查询ID
        /// </summary>
        public long Id { get; set; }
    }
}
