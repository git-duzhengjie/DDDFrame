namespace Infra.Core.Abstract
{
    public abstract class InputDTOBase : FrameobjectBase, IInputDTO
    {
        public bool IsNew { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long Id { get; set; }    
    }
}
