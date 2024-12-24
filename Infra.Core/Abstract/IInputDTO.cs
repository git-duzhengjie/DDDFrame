namespace Infra.Core.Abstract
{
    public interface IInputDTO:Iobject
    {
        /// <summary>
        /// 是否为添加，否则为修改
        /// </summary>
        bool IsNew { get; set; }

        /// <summary>
        /// 对象id
        /// </summary>
        public long Id { get; set; }
    }
}
