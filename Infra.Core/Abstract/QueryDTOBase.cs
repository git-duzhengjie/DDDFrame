namespace Infra.Core.Abstract
{
    public abstract class QueryDTOBase : FrameObjectBase, IQueryDTO
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
        /// 查询ID，用于区分查询对象
        /// </summary>
        public long Id { get; set; }
    }
}
