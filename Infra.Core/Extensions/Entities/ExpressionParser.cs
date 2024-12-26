using Infra.Core.Enums;
using Infra.Core.Models;
using System.Linq.Expressions;

namespace Infra.Core.Extensions.Entities
{
    public class ExpressionParser<TEntity> : IExpressionParser<TEntity>
    {
        public ParameterExpression Parameter { get; } = Expression.Parameter(typeof(TEntity));
        public Expression<Func<TEntity, bool>> ParserConditions(IEnumerable<Condition> conditions)
        {
            //将条件转化成表达是的Body
            var query = ParseExpressionBody(conditions);
            return Expression.Lambda<Func<TEntity, bool>>(query, Parameter);
        }

        public Expression<Func<TEntity, bool>> ParseExCondition(Condition condition)
        {
            var expression = ParseCondition(condition);
            return Expression.Lambda<Func<TEntity, bool>>(expression);
        }

        private Expression ParseExpressionBody(IEnumerable<Condition> conditions)
        {
            if (conditions == null || !conditions.Any())
            {
                return Expression.Constant(true, typeof(bool));
            }
            else if (conditions.Count() == 1)
            {
                return ParseCondition(conditions.First());
            }
            else
            {
                Expression left = ParseCondition(conditions.First());
                Expression right = ParseExpressionBody(conditions.Skip(1));
                return Expression.AndAlso(left, right);
            }
        }

        private Expression ParseCondition(Condition condition)
        {
            Expression key = Expression.Property(Parameter, condition.Key);
            //通过Tuple元组，实现Sql参数化。
            Expression value = ToTuple(condition.Value, key.Type);

            switch (condition.QuerySymbol)
            {
                case ConditionSymbol.Contains:
                    var values=condition.Value.ToString().Split(',');
                    Expression result=null;
                    foreach(var v in values)
                    {
                        var ve = ToTuple(v,key.Type);
                        if (result == null)
                        {
                            result = Expression.Call(key, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), ve);
                        }
                        else
                        {
                            result = Expression.Or(result, Expression.Call(key, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), ve));
                        }
                    }
                    return result;
                case ConditionSymbol.Equal:
                    return Expression.Equal(key, value);
                case ConditionSymbol.Greater:
                    return Expression.GreaterThan(key, value);
                case ConditionSymbol.GreaterEqual:
                    return Expression.GreaterThanOrEqual(key, value);
                case ConditionSymbol.Less:
                    return Expression.LessThan(key, value);
                case ConditionSymbol.LessEqual:
                    return Expression.LessThanOrEqual(key, value);
                case ConditionSymbol.NotEqual:
                    return Expression.NotEqual(key, value);
                case ConditionSymbol.In:
                    return ParserIn(condition);
                case ConditionSymbol.Between:
                    return ParserBetween(condition);
                default:
                    throw new NotImplementedException("不支持此操作。");
            }
        }

        private Expression ParserBetween(Condition conditions)
        {
            Expression key = Expression.Property(Parameter, conditions.Key);
            var valueArr = conditions.Value.ToString().Split(',');
            if (valueArr.Length != 2)
            {
                throw new NotImplementedException("ParserBetween参数错误");
            }

            Expression expression = Expression.Constant(true, typeof(bool));
            if (double.TryParse(valueArr[0], out double v1)
                && double.TryParse(valueArr[1], out double v2))
            {
                Expression startValue = ToTuple(v1, typeof(double));
                Expression start = Expression.GreaterThanOrEqual(key, Expression.Convert(startValue, key.Type));
                Expression endValue = ToTuple(v2, typeof(double));
                Expression end = Expression.LessThanOrEqual(key, Expression.Convert(endValue, key.Type));
                return Expression.AndAlso(start, end);
            }
            else if (DateTime.TryParse(valueArr[0], out DateTime v3)
                && DateTime.TryParse(valueArr[1], out DateTime v4))
            {
                Expression startValue = ToTuple(v3, typeof(DateTime));
                Expression start = Expression.GreaterThanOrEqual(key, Expression.Convert(startValue, key.Type));
                Expression endValue = ToTuple(v4, typeof(DateTime));
                Expression end = Expression.LessThanOrEqual(key, Expression.Convert(endValue, key.Type));
                return Expression.AndAlso(start, end);
            }
            else
            {
                throw new NotImplementedException("ParserBetween参数错误");
            }
        }

        private Expression ParserIn(Condition conditions)
        {
            Expression key = Expression.Property(Parameter, conditions.Key);
            var valueArr = conditions.Value.ToString().Split(',');
            Expression expression = Expression.Constant(false, typeof(bool));
            foreach (var itemVal in valueArr)
            {
                Expression value = ToTuple(itemVal, typeof(string));
                Expression right = Expression.Equal(key, Expression.Convert(value, key.Type));
                expression = Expression.Or(expression, right);
            }
            return expression;
        }

        private Expression ToTuple(object value, Type type)
        {
            var tuple = Tuple.Create(value);
            return Expression.Convert(
                 Expression.Property(Expression.Constant(tuple), nameof(tuple.Item1))
                 , type);
        }

    }
}
