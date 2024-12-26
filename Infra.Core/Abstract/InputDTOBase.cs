namespace Infra.Core.Abstract
{
    public abstract class InputDTOBase : FrameObjectBase, IInputDTO
    {
        public bool IsNew { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long Id { get; set; }    
    }
}
