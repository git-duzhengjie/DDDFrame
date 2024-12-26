using Infra.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Core.Models
{
    /// <summary>
    /// 条件实体。
    /// </summary>
    public class Condition
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ConditionSymbol QuerySymbol { get; set; }

        public static Condition GetPrimaryKeyCondition<T>(T t)
        {
            var key = typeof(T).Name + "Id";
            var value = typeof(T).GetProperty(key).GetValue(t);
            return new Condition { Key = key, Value = value, QuerySymbol = ConditionSymbol.Equal };
        }
    }
}
