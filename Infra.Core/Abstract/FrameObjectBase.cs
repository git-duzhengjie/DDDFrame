
namespace Infra.Core.Abstract
{
    public abstract class FrameObjectBase : IObject
    {
        /// <summary>
        /// 对象名
        /// </summary>
        public string ObjectName => GetType().Name;

        /// <summary>
        /// 对象类型
        /// </summary>
        public abstract int ObjectType { get; }
    }
}
