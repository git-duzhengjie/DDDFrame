namespace Infra.Core.Abstract
{
    public interface IPageQueryDTO:IQueryDTO
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
