namespace Infra.Core.Abstract
{
    public abstract class FrameobjectBase : Iobject
    {
        /// <summary>
        /// 对象名
        /// </summary>
        public string objectName => GetType().Name;

        /// <summary>
        /// 对象类型
        /// </summary>
        public abstract int objectType { get; }
    }
}
