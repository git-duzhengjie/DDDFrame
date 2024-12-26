namespace Infra.Core.Attributes
{
    public class InjectionAttribute:Attribute
    {
        /// <summary>
        /// 注入优先级
        /// </summary>
        public int Priority { get; set; }
    }
}
