using Infra.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Core.Extensions.Entities
{
    public interface IExpressionParser<T>
    {
        Expression<Func<T, bool>> ParserConditions(IEnumerable<Condition> conditions);
    }
}
