using Infra.Core.Enums;

namespace Infra.Core.Attributes
{
    public class ConditionAttribute:Attribute
    {
        public ConditionSymbol Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 是否加入或运算
        /// </summary>
        public bool Or { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool OrAnd { get; set; }
    }
}
