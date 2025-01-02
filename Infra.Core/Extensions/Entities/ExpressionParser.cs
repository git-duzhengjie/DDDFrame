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
            var ors=conditions.Where(x=>x.Or).ToArray();
            var ands=conditions.Except(ors).ToArray();
            Expression andExpression= ParseAnds(ands);
            Expression orExpression = ParseOrs(ors);
            return Expression.AndAlso(andExpression, orExpression);
        }

        private Expression ParseOrs(Condition[] ors)
        {
            if (ors == null || ors.Length == 0)
            {
                return Expression.Constant(true, typeof(bool));
            }
            else if (ors.Length == 1)
            {
                return ParseCondition(ors.First());
            }
            else
            {
                Expression left = ParseCondition(ors.First());
                Expression right = ParseExpressionBody(ors.Skip(1));
                return Expression.OrElse(left, right);
            }
        }

        private Expression ParseAnds(Condition[] ands)
        {
            if (ands == null || ands.Length == 0)
            {
                return Expression.Constant(true, typeof(bool));
            }
            else if (ands.Length == 1)
            {
                return ParseCondition(ands.First());
            }
            else
            {
                Expression left = ParseCondition(ands.First());
                Expression right = ParseExpressionBody(ands.Skip(1));
                return Expression.AndAlso(left, right);
            }
        }

        private Expression ParseCondition(Condition condition)
        {
            Expression key = ToKey(condition.Key,out var originType);
            //通过Tuple元组，实现Sql参数化。
            Expression value = ToValue(condition.Value, originType);

            switch (condition.QuerySymbol)
            {
                case ConditionSymbol.Contains:
                    var values=condition.Value.ToString().Split(',');
                    Expression result=null;
                    foreach(var v in values)
                    {
                        var ve = ToValue(v,key.Type);
                        if (result == null)
                        {
                            result = Expression.Call(key, typeof(string).GetMethod("Contains", [typeof(string)]), ve);
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

        private Expression ToKey(string key,out Type originType)
        {
            var keyExpression= Expression.Property(Parameter, key);
            originType = keyExpression.Type;
            if (keyExpression.Type.IsEnum)
            {
                return Expression.Convert(keyExpression, typeof(int));
            }
            return keyExpression;
        }

        private Expression ParserBetween(Condition conditions)
        {
            Expression key = ToKey(conditions.Key,out var originType);
            var valueArr = conditions.Value.ToString().Split(',');
            if (valueArr.Length != 2)
            {
                throw new NotImplementedException("ParserBetween参数错误");
            }

            Expression expression = Expression.Constant(true, typeof(bool));
            if (double.TryParse(valueArr[0], out double v1)
                && double.TryParse(valueArr[1], out double v2))
            {
                Expression startValue = ToValue(v1, typeof(double));
                Expression start = Expression.GreaterThanOrEqual(key, startValue);
                Expression endValue = ToValue(v2, typeof(double));
                Expression end = Expression.LessThanOrEqual(key, endValue);
                return Expression.AndAlso(start, end);
            }
            else if (DateTime.TryParse(valueArr[0], out DateTime v3)
                && DateTime.TryParse(valueArr[1], out DateTime v4))
            {
                Expression startValue = ToValue(v3, typeof(DateTime));
                Expression start = Expression.GreaterThanOrEqual(key, startValue);
                Expression endValue = ToValue(v4, typeof(DateTime));
                Expression end = Expression.LessThanOrEqual(key, endValue);
                return Expression.AndAlso(start, end);
            }
            else
            {
                throw new NotImplementedException("ParserBetween参数错误");
            }
        }

        private Expression ParserIn(Condition conditions)
        {
            Expression key = ToKey(conditions.Key,out var originType);
            var valueArr = conditions.Value.ToString().Split(',');
            Expression expression = Expression.Constant(false, typeof(bool));
            foreach (var itemVal in valueArr)
            {
                Expression value = ToValue(itemVal, typeof(string));
                Expression right = Expression.Equal(key, value);
                expression = Expression.Or(expression, right);
            }
            return expression;
        }

        private Expression ToValue(object value, Type type)
        {
            if (type.IsEnum)
            {
                return Expression.Constant((int)value, typeof(int));
            }
            return Expression.Constant(value, type);
            //var tuple = Tuple.Create(value);
            //if (type.IsEnum)
            //{
            //    return Expression.Constant(Enum.Parse(type,value))
            //}
            //if (type.IsEnum)
            //{
            //    return Expression.Convert(
            //     Expression.Property(Expression.Constant(tuple), nameof(tuple.Item1))
            //     , type);
            //}
            //return Expression.Convert(
            //     Expression.Property(Expression.Constant(tuple), nameof(tuple.Item1))
            //     , type);
        }

    }
}
