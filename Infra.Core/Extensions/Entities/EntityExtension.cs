using Infra.Core.Abstract;
using Infra.Core.Attributes;
using Infra.Core.Enums;
using Infra.Core.Extensions.Entities;
using Infra.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Core.Extensions.Entities
{
    public static class EntityExtension
    {
        public static Expression<Func<TEntity, bool>> GetPrimaryKeyFilter<TEntity>(this TEntity tEntity)
        {
            Condition condition = Condition.GetPrimaryKeyCondition(tEntity);
            IExpressionParser<TEntity> parser = new ExpressionParser<TEntity>();
            return parser.ParserConditions(new List<Condition> { condition });
        }

        public static Expression<Func<TEntity, bool>> GetPrimaryKeyFilter<TEntity>(object value)
        {
            var condition = new Condition
            {
                Key = typeof(TEntity).Key(),
                Value = value,
                QuerySymbol = ConditionSymbol.Equal,
            };
            IExpressionParser<TEntity> parser = new ExpressionParser<TEntity>();
            return parser.ParserConditions(new List<Condition> { condition });
        }

        public static Expression<Func<TEntity, bool>> GetPrimaryKeyFilterIn<TEntity>(object value)
        {
            var condition = new Condition
            {
                Key = typeof(TEntity).Key(),
                Value = value,
                QuerySymbol = ConditionSymbol.In
            };
            IExpressionParser<TEntity> parser = new ExpressionParser<TEntity>();
            return parser.ParserConditions(new List<Condition> { condition });
        }
        public static Expression<Func<TEntity, bool>> GetExpressionFilter<TEntity>(this object t)
        {
            var conditions = new List<Condition>();
            foreach (var property in t.GetType().GetProperties())
            {
                if (property.GetValue(t) != null && property.GetValue(t).ToString().IsNotNullOrEmpty())
                {
                    var attribute = Attribute.GetCustomAttribute(property, typeof(ConditionAttribute)) as ConditionAttribute;
                    if (attribute == null)
                    {
                        continue;
                    }
                    ConditionSymbol symbol=attribute.Name;
                    if (symbol.Equals(ConditionSymbol.NotMapped))
                        continue;
                    conditions.Add(new Condition
                    {
                        Key = attribute.Key,
                        Value = property.GetValue(t),
                        QuerySymbol = symbol,
                        Or=attribute.Or
                    });
                }
            }
            IExpressionParser<TEntity> parser = new ExpressionParser<TEntity>();
            return parser.ParserConditions(conditions);
        }

        public static Expression<Func<T, TKey>> SortLambda<T, TKey>(string defaultSort, string sort)
        {
            //1.创建表达式参数（指定参数或变量的类型:p）  
            var param = Expression.Parameter(typeof(T), "t");
            //2.构建表达式体(类型包含指定的属性:p.Name)  
            var body = Expression.Property(param, string.IsNullOrEmpty(sort) ? defaultSort : sort);
            //3.根据参数和表达式体构造一个lambda表达式  
            return Expression.Lambda<Func<T, TKey>>(Expression.Convert(body, typeof(TKey)), param);
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query, string propertyName)
        {
            return _OrderBy(query, propertyName, false);
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> query, string propertyName)
        {
            return _OrderBy(query, propertyName, true);
        }

        private static IOrderedQueryable<T> _OrderBy<T>(IQueryable<T> query, string propertyName, bool isDesc)
        {
            string methodname = isDesc ? "OrderByDescendingInternal" : "OrderByInternal";

            var memberProp = typeof(T).GetProperty(propertyName);
            if (memberProp == null)
                throw new Exception($"排序字段{propertyName}不存在");

            var method = typeof(QueryableExtension).GetMethod(methodname)
                .MakeGenericMethod(typeof(T), memberProp.PropertyType);

            return (IOrderedQueryable<T>)method.Invoke(null, new object[] { query, memberProp });
        }
        private static Expression<Func<T, TProp>> _GetLamba<T, TProp>(PropertyInfo memberProperty)
        {
            if (memberProperty.PropertyType != typeof(TProp)) throw new Exception();

            var thisArg = Expression.Parameter(typeof(T));
            var lamba = Expression.Lambda<Func<T, TProp>>(Expression.Property(thisArg, memberProperty), thisArg);

            return lamba;
        }



        public static class QueryableExtension
        {
            public static IOrderedQueryable<T> OrderByInternal<T, TProp>(IQueryable<T> query, PropertyInfo memberProperty)
            {//public
                return query.OrderBy(_GetLamba<T, TProp>(memberProperty));
            }

            public static IOrderedQueryable<T> OrderByDescendingInternal<T, TProp>(IQueryable<T> query, PropertyInfo memberProperty)
            {//public
                return query.OrderByDescending(_GetLamba<T, TProp>(memberProperty));
            }


        }
    }
}
