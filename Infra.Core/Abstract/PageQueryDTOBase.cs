namespace Infra.Core.Abstract
{
    public abstract class PageQueryDTOBase : QueryDTOBase, IPageQueryDTO
    {
        /// <summary>
        /// 
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// 
        /// </summary>
        public int Count { get; set; } = 10;
    }
}
