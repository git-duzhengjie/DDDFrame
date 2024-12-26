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
    }
}
