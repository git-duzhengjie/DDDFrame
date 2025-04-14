using Infra.Core.Abstract;
using Infra.Core.Json;
using Infra.EF.Attributes;
using Infra.EF.Context;
using Infra.EF.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SharpCompress.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infra.EF.Service
{
    /// <summary>
    /// 
    /// </summary>
    public class MongoDbData(FrameMongoDbContext dbContext) : IDbData
    {
        #region <常量>
        #endregion <常量>

        #region <变量>
        private readonly FrameMongoDbContext dbContext = dbContext;
        private static readonly Dictionary<string, string> s_navigaitonTypeTableMap = [];

        #endregion <变量>
        #region <构造方法和析构方法>
        #endregion <构造方法和析构方法>

        #region 自定义类

        #endregion

        #region <方法>
        /// 获取对象的序列化对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetJson(object obj)
        {
            return System.Text.Json.JsonSerializer.Serialize(obj);

        }

        /// <summary>
        /// 从数据库删除
        /// </summary>
        /// <param name="entities"></param>
        public async Task DeleteAsync(EntityBase[] entities)
        {
            var groups= entities.GroupBy(x => x.GetType());
            foreach(var gr in groups)
            {
                var method = dbContext.GetMethod("DeleteAsync");
                method = method.MakeGenericMethod(gr.Key);
                await (method.Invoke(dbContext, [gr.ToArray()]) as Task);
            }
        }

        struct ManyToManyNavigationTable
        {
            /// <summary>
            /// 表名
            /// </summary>
            public string TableName { get; set; }

            /// <summary>
            /// 关联key
            /// </summary>
            public string Key { get; set; }

            /// <summary>
            /// 当前表key
            /// </summary>
            public string ThisKey { get; set; }

            /// <summary>
            /// 关联值
            /// </summary>
            public object Value { get; set; }

            /// <summary>
            /// 关联值
            /// </summary>
            public List<object[]> Values { get; set; }

            /// <summary>
            /// 另外一个表的key
            /// </summary>
            public string OtherKey { get; set; }
        }

        struct ManyToOneNavigationTable
        {
            /// <summary>
            /// 表名
            /// </summary>
            public string TableName { get; set; }

            /// <summary>
            /// 关联key
            /// </summary>
            public string Key { get; set; }

            /// <summary>
            /// 关联值
            /// </summary>
            public object Value { get; set; }

            /// <summary>
            /// 当前值
            /// </summary>
            public object CurrentValue { get; set; }
        }

        struct RefTable
        {
            /// <summary>
            /// 表名
            /// </summary>
            public string TableName { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public Dictionary<string, object> KeyValues { get; set; }

            /// <summary>
            /// 执行优先级
            /// </summary>
            public int Priority { get; set; }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public async Task SaveAsync()
        {
            await dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 更新到数据库
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="updateNavigation">是否更新导航属性</param>
        /// <returns></returns>
        public async Task UpdateAsync(bool updateNavigation, params EntityBase[] entities)
        {
            var groups = entities.GroupBy(x => x.GetType());
            foreach(var gr in groups)
            {
                var method = dbContext.GetType().GetMethods()
                    .FirstOrDefault(x=>x.Name=="UpdateAsync"&&x.GetParameters()
                    .Any(p=>p.ParameterType.IsArray&&p.ParameterType.GetElementType().IsAssignableTo(typeof(EntityBase))));
                method = method.MakeGenericMethod(gr.Key);
                await (method.Invoke(dbContext, [gr.ToArray()]) as Task);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entitiesWithProperties"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task UpdateAsync(params UpdateData[] entitiesWithProperties)
        {
            var groups = entitiesWithProperties.GroupBy(x => x.Update.GetType());
            foreach (var gr in groups)
            {
                var method = dbContext.GetType().GetMethods()
                    .FirstOrDefault(x => x.Name == "UpdateAsync" && x.GetParameters()
                    .Any(p => p.ParameterType.IsArray && p.ParameterType.GetElementType()==typeof(UpdateData)));
                method = method.MakeGenericMethod(gr.Key);
                await (method.Invoke(dbContext, [gr.ToArray()]) as Task);
            }
        }


        /// <summary>
        /// 判断typeA中是否存在导航到typeB的导航属性
        /// </summary>
        /// <param name="typeA"></param>
        /// <param name="typeB"></param>
        /// <returns></returns>
        private static bool ExistManyToManyNavigation(Type typeA, Type typeB)
        {
            var properties = typeA.GetProperties();
            foreach (var property in properties)
            {
                //排除掉只读只写以及notmapped和导航属性的数据
                if ((!property.CanWrite
                    || !property.CanRead)
                    || property.GetCustomAttribute<NotMappedAttribute>() != null)
                    continue;
                if (property.GetAccessors().Length < 2)
                {
                    continue;
                }
                if (property.PropertyType.IsGenericType && ArrayType(property.PropertyType.GetGenericTypeDefinition()))
                {
                    var type = property.PropertyType.GetGenericArguments()[0];
                    if (type == typeB)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 判断typeA中是否存在导航到typeB的导航属性
        /// </summary>
        /// <param name="typeA"></param>
        /// <param name="typeB"></param>
        /// <returns></returns>
        private static bool ExistManyToOneNavigation(Type typeA, Type typeB)
        {
            return !ExistManyToManyNavigation(typeA, typeB);
        }

        /// <summary>
        /// 是否为数组类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool ArrayType(Type type)
        {
            return type == typeof(ICollection<>) || type == typeof(IList<>) || type == typeof(IEnumerable<>) || type == typeof(List<>);
        }

        private string GetKey(string type1, string type2)
        {
            var strs = new List<string> { type1, type2 };
            strs = strs.OrderBy(x => x).ToList();
            return "".Join(strs);
        }

        private static string GetTableName(EntityBase table)
        {
            var tableName = table.GetType().Name.ToLower();
            switch (tableName)
            {
                case "window":
                    return "\"window\"";
                case "column":
                    return "\"column\"";
                case "user":
                    return "\"user\"";
                default:
                    return tableName;
            }
        }

        /// <summary>
        /// 获取表名、表字段等内容
        /// </summary>
        /// <param name="table"></param>
        /// <param name="refTables"></param>
        /// <param name="manyToManyNavigationTables"></param>
        /// <param name="manyToOneNavigationTables"></param>
        /// <param name="updateNavigation"></param>
        /// <param name="priority"></param>
        /// <param name="propertyNames"></param>
        private async Task GetTableDataAsync(EntityBase table, List<RefTable> refTables,
            List<ManyToManyNavigationTable> manyToManyNavigationTables,
            List<ManyToOneNavigationTable> manyToOneNavigationTables,
            bool updateNavigation,
            int priority = 0,
            string[] propertyNames = null)
        {
            var refTable = new RefTable();
            refTable.Priority = priority;
            refTable.KeyValues = new Dictionary<string, object>();
            refTable.TableName = GetTableName(table);
            var properties = table.GetType().GetProperties();
            if (propertyNames != null)
            {
                properties = properties.Where(x => propertyNames.Any(p => p.ToLower() == x.Name.ToLower()) || x.Name.ToLower() == "id").ToArray();
            }
            var efProperties = dbContext.Model.FindEntityType(table.GetType()).GetProperties();
            foreach (var property in properties)
            {
                try
                {
                    //排除掉只读只写以及notmapped和导航属性的数据
                    if ((!property.CanWrite
                        || !property.CanRead)
                        || property.GetCustomAttribute<NotMappedAttribute>() != null)
                        continue;
                    if (property.GetAccessors().Length < 2)
                    {
                        continue;
                    }
                    if (property.PropertyType.IsGenericType && ArrayType(property.PropertyType.GetGenericTypeDefinition()) && property.GetCustomAttribute<FrameJsonAttribute>() == null)
                    {
                        var type = property.PropertyType.GetGenericArguments()[0];
                        if (type.IsSubclassOf(typeof(EntityBase)))
                        {
                            if (updateNavigation)
                            {
                                if (ExistManyToManyNavigation(type, table.GetType()))
                                {
                                    var navigationTable = new ManyToManyNavigationTable();
                                    var key = GetKey(table.GetType().Name.ToLower(), type.Name.ToLower());
                                    if (!s_navigaitonTypeTableMap.TryGetValue(key, out var navigationTableName))
                                    {
                                        navigationTableName = $"{table.GetType().Name.ToLower()}{type.Name.ToLower()}";
                                        var r = await dbContext.Database.ExecuteSqlRawAsync($"update pg_class set relname=relname where  relname='{navigationTableName}'");
                                        if (r <= 0)
                                        {
                                            navigationTableName = $"{type.Name.ToLower()}{table.GetType().Name.ToLower()}";
                                        }
                                        s_navigaitonTypeTableMap.TryAdd(key, navigationTableName);
                                    }
                                    navigationTable.TableName = navigationTableName;
                                    navigationTable.ThisKey = $"{table.GetType().Name.ToLower()}";
                                    navigationTable.Value = table.Id;
                                    navigationTable.Key = $"{table.GetType().Name.ToLower()}sid,{type.Name.ToLower()}sid";
                                    navigationTable.Values = new List<object[]>();
                                    var value = property.GetValue(table);
                                    if (value != null)
                                    {
                                        ICollection objects = (ICollection)value;
                                        navigationTable.Values = new List<object[]>();
                                        foreach (var obj in objects)
                                        {
                                            if (obj == null)
                                            {
                                                continue;
                                            }
                                            navigationTable.Values.Add(new object[] { table.Id, (obj as EntityBase).Id });
                                        }
                                    }
                                    navigationTable.OtherKey = type.Name.ToLower();
                                    if (navigationTable.Values.Any())
                                    {
                                        manyToManyNavigationTables.Add(navigationTable);
                                    }

                                }
                                if (ExistManyToOneNavigation(type, table.GetType()))
                                {
                                    var value = property.GetValue(table);
                                    if (value != null)
                                    {
                                        ICollection objects = (ICollection)value;
                                        foreach (var obj in objects)
                                        {
                                            if (obj == null)
                                            {
                                                continue;
                                            }
                                            manyToOneNavigationTables.Add(new ManyToOneNavigationTable
                                            {
                                                TableName = GetTableName(obj as EntityBase),
                                                Key = table.GetType().Name.ToString().ToLower() + "id",
                                                CurrentValue = (obj as EntityBase).Id,
                                                Value = table.Id
                                            });
                                        }
                                    }
                                }
                            }
                            continue;
                        }
                    }
                    if (property.PropertyType.IsSubclassOf(typeof(EntityBase)) && property.GetCustomAttribute<FrameJsonAttribute>() == null)
                    {
                        if (property.GetValue(table) != null)
                        {
                            var aggroot = (property.GetValue(table) as EntityBase);
                            var columnName = $"{property.Name.ToLower()}id";
                            if (!refTable.KeyValues.ContainsKey(columnName) &&
                                property.GetCustomAttribute<ForeignKeyAttribute>() == null)
                            {

                                if (efProperties.Any(x => x.Name.ToLower() == columnName))
                                {
                                    refTable.KeyValues.Add(columnName, aggroot.Id);
                                    priority += 1;
                                }
                                else
                                {
                                    refTable.Priority += 1;
                                }
                            }
                            if (!refTable.KeyValues.ContainsKey(columnName) &&
                                property.GetCustomAttribute<ForeignKeyAttribute>() != null)
                            {

                                var foreignKey = property.GetCustomAttribute<ForeignKeyAttribute>();
                                if (efProperties.Any(x => x.Name.ToLower() == foreignKey.Name.ToLower()))
                                {
                                    priority += 1;
                                }
                                else
                                {
                                    refTable.Priority += 1;
                                }
                            }
                            else if (refTable.KeyValues.ContainsKey(columnName))
                            {
                                var value = (long)refTable.KeyValues[columnName];
                                if (value == 0)
                                {
                                    refTable.KeyValues[columnName] = aggroot.Id;
                                }
                            }
                            if (updateNavigation)
                            {
                                await GetTableDataAsync(aggroot, refTables, manyToManyNavigationTables, manyToOneNavigationTables, false, priority);
                            }
                        }
                    }
                    else
                    {
                        if (!refTable.KeyValues.ContainsKey(property.Name.ToLower()))
                        {
                            if (property.GetCustomAttribute<FrameJsonAttribute>() != null)
                            {
                                refTable.KeyValues.Add(property.Name.ToLower(), GetJson(property.GetValue(table)));
                            }
                            else
                            {
                                refTable.KeyValues.Add(property.Name.ToLower(), property.GetValue(table));
                            }
                        }
                    }
                }
                catch
                {
                    throw;
                }

            }
            refTables.Add(refTable);
            //_tableMap.Add(table.GetType(), (refTables, navigationTables));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="entitites"></param>
        /// <param name="updateNavigation">是否更新导航属性</param>
        /// <returns></returns>
        public async Task AddAsync(bool updateNavigation, params EntityBase[] entitites)
        {
            var groups = entitites.GroupBy(x => x.GetType());
            foreach (var gr in groups)
            {
                await dbContext.AddAsync(gr.ToArray());
            }
        }

        #endregion <方法>

        #region <事件>
        #endregion <事件>
    }
}
