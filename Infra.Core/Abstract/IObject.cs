namespace Infra.Core.Abstract
{
    public interface Iobject
    {
        /// <summary>
        /// 对象名
        /// </summary>
        string objectName { get; }

        /// <summary>
        /// 对象类型
        /// </summary>
        int objectType { get; }
    }
}
