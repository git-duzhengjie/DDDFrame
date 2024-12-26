namespace Infra.Core.Abstract
{
    public abstract class PageQueryDTOBase : QueryDTOBase, IPageQueryDTO
    {
        /// <summary>
        /// 
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Count { get; set; }
    }
}
