using Infra.Core.Enums;
using Infra.Core.Models;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection.Metadata;

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

        private BinaryExpression ParseExpressionBody(IEnumerable<Condition> conditions)
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
                var ands=ors.Where(x=>x.OrAnd).ToArray();
                var os = ors.Except(ands).ToArray();
                Expression left;
                Expression right;
                if (ands.Length != 0)
                {
                    left = ParseAnds(ands);
                    right = ParseOrs(os);
                }
                else
                {
                    left=ParseCondition(ors.First());
                    right=ParseOrs(os.Skip(1).ToArray());
                }
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
                Expression right = ParseAnds(ands.Skip(1).ToArray());
                return Expression.AndAlso(left, right);
            }
        }

        private Expression ParseCondition(Condition condition)
        {
            Expression key = ToKey(condition.Key,out var originType);
            //通过Tuple元组，实现Sql参数化。
            switch (condition.QuerySymbol)
            {
                case ConditionSymbol.NotContains:
                    var values = condition.Value.ToString().Split(',');
                    Expression result = null;
                    foreach (var v in values)
                    {
                        var ve = ToValue(v, key.Type);
                        if (result == null)
                        {
                            result = Expression.Call(key, typeof(string).GetMethod("Contains", [typeof(string)]), ve);
                        }
                        else
                        {
                            result = Expression.Or(result, Expression.Call(key, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), ve));
                        }
                    }
                    return Expression.Not(result);
                case ConditionSymbol.NotContainsAll:
                    values = condition.Value.ToString().Split(',');
                    result = null;
                    foreach (var v in values)
                    {
                        var ve = ToValue(v, key.Type);
                        if (result == null)
                        {
                            result = Expression.Call(key, typeof(string).GetMethod("Contains", [typeof(string)]), ve);
                        }
                        else
                        {
                            result = Expression.And(result, Expression.Call(key, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), ve));
                        }
                    }
                    return Expression.Not(result);
                case ConditionSymbol.Contains:
                    values=condition.Value.ToString().Split(',');
                    result=null;
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
                case ConditionSymbol.ContainsAll:
                    values = condition.Value.ToString().Split(',');
                    result = null;
                    foreach (var v in values)
                    {
                        var ve = ToValue(v, key.Type);
                        if (result == null)
                        {
                            result = Expression.Call(key, typeof(string).GetMethod("Contains", [typeof(string)]), ve);
                        }
                        else
                        {
                            result = Expression.And(result, Expression.Call(key, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), ve));
                        }
                    }
                    return result;
                case ConditionSymbol.Equal:
                    Expression value = ToValue(condition.Value, originType);
                    return Expression.Equal(key, value);
                case ConditionSymbol.Equals:
                    if(condition.Value is IList list)
                    {
                        result = null;
                        foreach (var v in list)
                        {
                            value = ToValue(v, originType);
                            if (result == null)
                            {
                                result = Expression.Equal(key, value);
                            }
                            else
                            {
                                result = Expression.Or(result, Expression.Equal(key, value));
                            }
                        }
                        return result;
                    }
                    return null;
                case ConditionSymbol.NotEquals:
                    if (condition.Value is IList list2)
                    {
                        result = null;
                        foreach (var v in list2)
                        {
                            value = ToValue(v, originType);
                            if (result == null)
                            {
                                result = Expression.Equal(key, value);
                            }
                            else
                            {
                                result = Expression.And(result, Expression.Equal(key, value));
                            }
                        }
                        return Expression.Not(result);
                    }
                    return null;
                case ConditionSymbol.Greater:
                    value = ToValue(condition.Value, originType);
                    return Expression.GreaterThan(key, value);
                case ConditionSymbol.GreaterEqual:
                    value = ToValue(condition.Value, originType);
                    return Expression.GreaterThanOrEqual(key, value);
                case ConditionSymbol.GreaterEqualOrNone:
                    value = ToValue(condition.Value, originType);
                    return Expression.Or(Expression.Equal(key, Expression.Constant(null, typeof(object))), Expression.GreaterThanOrEqual(key, value));
                case ConditionSymbol.Less:
                    value = ToValue(condition.Value, originType);
                    return Expression.LessThan(key, value);
                case ConditionSymbol.LessEqual:
                    value = ToValue(condition.Value, originType);
                    return Expression.LessThanOrEqual(key, value);
                case ConditionSymbol.NotEqual:
                    value = ToValue(condition.Value, originType);
                    return Expression.NotEqual(key, value);
                case ConditionSymbol.In:
                    return ParserIn(condition);
                case ConditionSymbol.NotIn:
                    return Expression.Not(ParserIn(condition));
                case ConditionSymbol.Between:
                    return ParserBetween(condition);
                case ConditionSymbol.CountGreaterEqual:
                    return Expression.GreaterThanOrEqual(Expression.ArrayLength(key), Expression.Constant(condition.Value,typeof(int)));
                case ConditionSymbol.CountEqual:
                    return Expression.Equal(Expression.ArrayLength(key), Expression.Constant(condition.Value, typeof(int)));
                default:
                    throw new NotImplementedException("不支持此操作。");
            }
        }

        private Expression ToKey(string key,out Type originType)
        {
            var keyExpression= Expression.Property(Parameter, key);
            originType = keyExpression.Type;
            if (keyExpression.Type.IsEnum||(keyExpression.Type.IsNullableType(out var valueType)&&valueType.IsEnum))
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
                Expression value = ToValue(Convert.ChangeType(itemVal,originType), originType);
                Expression right = Expression.Equal(key, value);
                expression = Expression.Or(expression, right);
            }
            return expression;
        }

        private static Expression ToValue(object value, Type type)
        {
            if (type.IsEnum || (type.IsNullableType(out var valueType)&&valueType.IsEnum))
            {
                return Expression.Constant((int)value, typeof(int));
            }
            return Expression.Constant(value, type);
        }

    }
}
