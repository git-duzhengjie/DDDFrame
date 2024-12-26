namespace Infra.Core.Abstract
{
    public abstract class QueryDTOBase : FrameObjectBase, IQueryDTO
    {
        public string Order { get; set; }
        public bool OrderDesc { get; set; }
    }
}
