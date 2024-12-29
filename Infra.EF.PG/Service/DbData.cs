using Infra.Core.Abstract;
using Infra.Core.Json;
using Infra.EF.PG.Attributes;
using Infra.EF.PG.Context;
using Infra.EF.PG.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infra.EF.PG.Service
{
    public class DbData:IDbData
    {
        #region <常量>
        #endregion <常量>

        #region <变量>
        private readonly FrameDbContext _lougeDbContext;
        private static readonly Dictionary<string, string> s_navigaitonTypeTableMap = new();
        #endregion <变量>

        #region <属性>
        #endregion <属性>

        #region <构造方法和析构方法>
        /// <summary>
        /// 
        /// </summary>
        public DbData(FrameDbContext lougeDbContext)
        {
            _lougeDbContext = lougeDbContext;
        }
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
        /// <param name="entity"></param>
        public async Task DeleteAsync(EntityBase entity)
        {
            var tableName = GetTableName(entity);
            await _lougeDbContext.Database.ExecuteSqlRawAsync($"delete from {0} where id={1}",tableName,entity.Id);
        }

        /// <summary>
        /// 从数据库删除
        /// </summary>
        /// <param name="entities"></param>
        public async Task DeleteAsync(EntityBase[] entities)
        {
            var sql = "";
            foreach (var entity in entities)
            {
                var tableName = GetTableName(entity);
                sql += $"delete from {tableName} where id={entity.Id};";
            }
            if (sql.IsNotNullOrEmpty())
            {
                await _lougeDbContext.Database.ExecuteSqlRawAsync(sql);
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
            await _lougeDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 更新到数据库
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateNavigation">是否更新导航属性</param>
        /// <returns></returns>
        public async Task UpdateAsync(EntityBase entity, bool updateNavigation = false)
        {
            var refTables = new List<RefTable>();
            var properties = entity.GetProperties();
            List<ManyToManyNavigationTable> manyToManyNavigationTables = new List<ManyToManyNavigationTable>();
            List<ManyToOneNavigationTable> manyToOneNavigationTables = new List<ManyToOneNavigationTable>();
            await GetTableDataAsync(entity, refTables, manyToManyNavigationTables, manyToOneNavigationTables, updateNavigation);
            if (refTables.Any())
            {
                foreach (var refTable in refTables.OrderByDescending(x => x.Priority))
                {
                    if (!refTable.KeyValues.Any())
                    {
                        continue;
                    }
                    var keyValues = refTable.KeyValues.Where(x => x.Key != "id").ToList();
                    var tableName = refTable.TableName;
                    var sql = $"update {tableName} set ";
                    var keyArray = new List<string>();
                    for (var i = 0; i < keyValues.Count; i++)
                    {
                        var key = keyValues[i].Key;
                        keyArray.Add($"{key}={{{i}}}");
                    }
                    sql += $"{string.Join(",", keyArray)} where id={{{keyValues.Count}}}";
                    await _lougeDbContext.Database.ExecuteSqlRawAsync(sql, keyValues.Select(x => x.Value).Append(refTable.KeyValues.First(x => x.Key == "id").Value).ToArray());
                }
            }

            if (manyToManyNavigationTables.Any())
            {
                foreach (var navigationTable in manyToManyNavigationTables)
                {

                    var insertArray = new List<string>();
                    var index = 0;

                    navigationTable.Values.ForEach(s =>
                    {
                        insertArray.Add($"({{{index}}},{{{index + 1}}})");
                        index += 2;
                    });
                    var insertValue = ",".Join(insertArray);
                    var sql = $"insert into {navigationTable.TableName}({navigationTable.Key}) values{insertValue} ON CONFLICT({navigationTable.Key}) DO NOTHING";
                    var sqlRaw = $@"delete from {navigationTable.TableName} where {entity.GetType().Name.ToLower()}sid={entity.Id};";
                    await _lougeDbContext.Database.ExecuteSqlRawAsync(sqlRaw);
                    await _lougeDbContext.Database.ExecuteSqlRawAsync(sql, navigationTable.Values.SelectMany(x => x).ToArray());

                }
            }
            if (manyToOneNavigationTables.Any())
            {
                foreach (var manyToOneNavigationTable in manyToOneNavigationTables)
                {
                    var sql = $"update {manyToOneNavigationTable.TableName} set {manyToOneNavigationTable.Key}={{0}} where id={{1}}";
                    await _lougeDbContext.Database.ExecuteSqlRawAsync(sql, manyToOneNavigationTable.Value, manyToOneNavigationTable.CurrentValue);
                }
            }
        }

        /// <summary>
        /// 更新到数据库
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="updateNavigation">是否更新导航属性</param>
        /// <returns></returns>
        public async Task UpdateAsync(bool updateNavigation, params EntityBase[] entities)
        {
            var refTables = new List<RefTable>();
            List<ManyToManyNavigationTable> manyToManyNavigationTables = new List<ManyToManyNavigationTable>();
            List<ManyToOneNavigationTable> manyToOneNavigationTables = new List<ManyToOneNavigationTable>();
            foreach (var entity in entities)
            {
                await GetTableDataAsync(entity, refTables, manyToManyNavigationTables, manyToOneNavigationTables, updateNavigation);
            }
            if (refTables.Any())
            {
                var groups = refTables.GroupBy(x => (x.Priority, x.TableName));
                foreach (var group in groups)
                {
                    var refTable = group.First();
                    if (!refTable.KeyValues.Any())
                    {
                        continue;
                    }
                    var keyValues = refTable.KeyValues.Where(x => x.Key != "id").ToList();
                    var tableName = refTable.TableName;
                    var allSql = "";
                    for (var i = 0; i < group.Count(); i++)
                    {
                        var sql = $"update {tableName} set ";
                        var keyArray = new List<string>();
                        for (var j = 0; j < keyValues.Count; j++)
                        {
                            var key = keyValues[j].Key;
                            keyArray.Add($"{key}={{{j + i * (keyValues.Count + 1)}}}");
                        }
                        sql += $"{string.Join(",", keyArray)} where id={{{keyValues.Count + i * (keyValues.Count + 1)}}}";
                        allSql += sql + ";";
                    }
                    await _lougeDbContext.Database.ExecuteSqlRawAsync(allSql, group.SelectMany(x => x.KeyValues.Where(k => k.Key != "id").Select(k => k.Value).Append(x.KeyValues.First(k => k.Key == "id").Value)).ToArray());
                }
            }

            if (manyToManyNavigationTables.Any())
            {
                var insertSql = "";
                var count = 0;
                var deleteSql = "";
                foreach (var navigationTable in manyToManyNavigationTables)
                {

                    var insertArray = new List<string>();
                    var index = 0;

                    navigationTable.Values.ForEach(s =>
                    {
                        insertArray.Add($"({{{index + count}}},{{{index + 1 + count}}})");
                        index += 2;
                    });
                    count += index;
                    var insertValue = ",".Join(insertArray);
                    insertSql += $"insert into {navigationTable.TableName}({navigationTable.Key}) values{insertValue} ON CONFLICT({navigationTable.Key}) DO NOTHING;";
                    deleteSql += $@"delete from {navigationTable.TableName} where {navigationTable.ThisKey.ToLower()}sid={navigationTable.Value};";
                }
                await _lougeDbContext.Database.ExecuteSqlRawAsync(deleteSql);
                await _lougeDbContext.Database.ExecuteSqlRawAsync(insertSql, manyToManyNavigationTables.SelectMany(x => x.Values.SelectMany(v => v)).ToArray());
            }
            if (manyToOneNavigationTables.Any())
            {
                var allSql = "";
                var index = 0;
                foreach (var manyToOneNavigationTable in manyToOneNavigationTables)
                {
                    allSql += $"update {manyToOneNavigationTable.TableName} set {manyToOneNavigationTable.Key}={{{index * 2}}} where id={{{index * 2 + 1}}};";
                    index++;
                }
                await _lougeDbContext.Database.ExecuteSqlRawAsync(allSql, manyToOneNavigationTables.SelectMany(x => new List<object> { x.Value, x.CurrentValue }).ToArray());
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
            var refTables = new List<RefTable>();
            List<ManyToManyNavigationTable> manyToManyNavigationTables = new List<ManyToManyNavigationTable>();
            List<ManyToOneNavigationTable> manyToOneNavigationTables = new List<ManyToOneNavigationTable>();
            foreach (var entity in entitiesWithProperties)
            {
                await GetTableDataAsync(entity.Update, refTables, manyToManyNavigationTables, manyToOneNavigationTables, true, 0, entity.Properties);
            }
            if (refTables.Any())
            {
                var groups = refTables.GroupBy(x => (x.Priority, x.TableName));
                foreach (var group in groups)
                {
                    var refTable = group.First();
                    if (!refTable.KeyValues.Any())
                    {
                        continue;
                    }
                    var keyValues = refTable.KeyValues
                        .Where(x => x.Key != "id")
                        .ToList();
                    var tableName = refTable.TableName;
                    var allSql = "";
                    for (var i = 0; i < group.Count(); i++)
                    {
                        var sql = $"update {tableName} set ";
                        var keyArray = new List<string>();
                        for (var j = 0; j < keyValues.Count; j++)
                        {
                            var key = keyValues[j].Key;
                            keyArray.Add($"{key}={{{j + i * (keyValues.Count + 1)}}}");
                        }
                        sql += $"{string.Join(",", keyArray)} where id={{{keyValues.Count + i * (keyValues.Count + 1)}}}";
                        allSql += sql + ";";
                    }
                    await _lougeDbContext.Database.ExecuteSqlRawAsync(allSql, group.SelectMany(x => x.KeyValues.Where(k => k.Key != "id").Select(k => k.Value).Append(x.KeyValues.First(k => k.Key == "id").Value)).ToArray());

                }
            }

            if (manyToManyNavigationTables.Any())
            {
                var insertSql = "";
                var count = 0;
                var deleteSql = "";
                foreach (var navigationTable in manyToManyNavigationTables)
                {

                    var insertArray = new List<string>();
                    var index = 0;

                    navigationTable.Values.ForEach(s =>
                    {
                        insertArray.Add($"({{{index + count}}},{{{index + 1 + count}}})");
                        index += 2;
                    });
                    count += index;
                    var insertValue = ",".Join(insertArray);
                    insertSql += $"insert into {navigationTable.TableName}({navigationTable.Key}) values{insertValue} ON CONFLICT({navigationTable.Key}) DO NOTHING;";
                    deleteSql += $@"delete from {navigationTable.TableName} where {navigationTable.ThisKey.ToLower()}sid={navigationTable.Value};";
                }
                await _lougeDbContext.Database.ExecuteSqlRawAsync(deleteSql);
                await _lougeDbContext.Database.ExecuteSqlRawAsync(insertSql, manyToManyNavigationTables.SelectMany(x => x.Values.SelectMany(v => v)).ToArray());
            }
            if (manyToOneNavigationTables.Any())
            {
                var allSql = "";
                var index = 0;
                foreach (var manyToOneNavigationTable in manyToOneNavigationTables)
                {
                    allSql += $"update {manyToOneNavigationTable.TableName} set {manyToOneNavigationTable.Key}={{{index * 2}}} where id={{{index * 2 + 1}}};";
                    index++;
                }
                await _lougeDbContext.Database.ExecuteSqlRawAsync(allSql, manyToOneNavigationTables.SelectMany(x => new List<object> { x.Value, x.CurrentValue }).ToArray());
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
            var efProperties = _lougeDbContext.Model.FindEntityType(table.GetType()).GetProperties();
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
                                        var r = await _lougeDbContext.Database.ExecuteSqlRawAsync($"update pg_class set relname=relname where  relname='{navigationTableName}'");
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
        /// <param name="entity"></param>
        /// <param name="updateNavigation">是否更新导航属性</param>
        /// <returns></returns>
        public async Task AddAsync(EntityBase entity, bool updateNavigation = false)
        {
            var refTables = new List<RefTable>();
            var manyToManyNavigationTables = new List<ManyToManyNavigationTable>();
            var manyToOneNavigationTables = new List<ManyToOneNavigationTable>();
            await GetTableDataAsync(entity, refTables, manyToManyNavigationTables, manyToOneNavigationTables, updateNavigation);
            if (refTables.Any())
            {
                foreach (var refTable in refTables.OrderByDescending(x => x.Priority))
                {
                    if (!refTable.KeyValues.Any())
                    {
                        continue;
                    }
                    var valueParameters = new List<string>();
                    for (var i = 0; i < refTable.KeyValues.Count; i++)
                    {
                        valueParameters.Add($"{{{i}}}");
                    }
                    var tableName = refTable.TableName;
                    var sql = $"insert into {tableName}({string.Join(",", refTable.KeyValues.Select(x => x.Key))}) values({string.Join(",", valueParameters)}) ON CONFLICT(id) DO NOTHING";
                    await _lougeDbContext.Database.ExecuteSqlRawAsync(sql, refTable.KeyValues.Select(x => x.Value).ToArray());
                }
            }

            if (manyToManyNavigationTables.Any())
            {
                foreach (var navigationTable in manyToManyNavigationTables)
                {

                    var insertArray = new List<string>();
                    var index = 0;

                    navigationTable.Values.ForEach(s =>
                    {
                        insertArray.Add($"({{{index}}},{{{index + 1}}})");
                        index += 2;
                    });
                    var insertValue = ",".Join(insertArray);
                    var tableName = navigationTable.TableName;
                    var sql = $"insert into {navigationTable.TableName}({navigationTable.Key}) values{insertValue} ON CONFLICT({navigationTable.Key}) DO NOTHING";
                    await _lougeDbContext.Database.ExecuteSqlRawAsync(sql, navigationTable.Values.SelectMany(x => x).ToArray());
                }
            }

            if (manyToOneNavigationTables.Any())
            {
                string sql = "";
                foreach (var manyToOneNavigationTable in manyToOneNavigationTables)
                {
                    var tableName = manyToOneNavigationTable.TableName;
                    sql = $"update {tableName} set {manyToOneNavigationTable.Key}={manyToOneNavigationTable.Value} where id={manyToOneNavigationTable.CurrentValue};";
                    await _lougeDbContext.Database.ExecuteSqlRawAsync(sql);
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entitites"></param>
        /// <param name="updateNavigation">是否更新导航属性</param>
        /// <returns></returns>
        public async Task AddAsync(bool updateNavigation, params EntityBase[] entitites)
        {
            var refTables = new List<RefTable>();
            var manyToManyNavigationTables = new List<ManyToManyNavigationTable>();
            var manyToOneNavigationTables = new List<ManyToOneNavigationTable>();
            foreach (var entity in entitites)
            {
                await GetTableDataAsync(entity, refTables, manyToManyNavigationTables, manyToOneNavigationTables, updateNavigation);
            }

            if (refTables.Any())
            {
                var groups = refTables.GroupBy(x => (x.Priority, x.TableName));
                foreach (var group in groups.OrderByDescending(x => x.Key))
                {
                    var refTable = group.First();
                    if (!refTable.KeyValues.Any())
                    {
                        continue;
                    }
                    var valueParameterStrs = new List<string>();
                    for (var i = 0; i < group.Count(); i++)
                    {
                        var valueParameters = new List<string>();
                        for (var j = 0; j < refTable.KeyValues.Count; j++)
                        {
                            valueParameters.Add($"{{{i * refTable.KeyValues.Count + j}}}");
                        }
                        var str = $"({string.Join(",", valueParameters)})";
                        valueParameterStrs.Add(str);
                    }
                    var tableName = refTable.TableName;
                    var sql = $"insert into {tableName}({string.Join(",", refTable.KeyValues.Select(x => x.Key))}) values {string.Join(",", valueParameterStrs)} ON CONFLICT(id) DO NOTHING";
                    await _lougeDbContext.Database.ExecuteSqlRawAsync(sql, group.SelectMany(x => x.KeyValues.Select(k => k.Value)).ToArray());
                }
            }

            if (manyToManyNavigationTables.Any())
            {

                var insertSql = "";
                var count = 0;
                foreach (var navigationTable in manyToManyNavigationTables)
                {

                    var insertArray = new List<string>();
                    var index = 0;

                    navigationTable.Values.ForEach(s =>
                    {
                        insertArray.Add($"({{{index + count}}},{{{index + 1 + count}}})");
                        index += 2;
                    });
                    count += index;
                    var insertValue = ",".Join(insertArray);
                    var tableName = navigationTable.TableName;
                    insertSql += $"insert into {tableName}({navigationTable.Key}) values{insertValue} ON CONFLICT({navigationTable.Key}) DO NOTHING;";
                }
                await _lougeDbContext.Database.ExecuteSqlRawAsync(insertSql, manyToManyNavigationTables.SelectMany(x => x.Values.SelectMany(v => v)).ToArray());
            }

            if (manyToOneNavigationTables.Any())
            {
                var allSql = "";
                var index = 0;
                foreach (var manyToOneNavigationTable in manyToOneNavigationTables)
                {
                    var tableName = manyToOneNavigationTable.TableName;
                    allSql += $"update {tableName} set {manyToOneNavigationTable.Key}={{{index * 2}}} where id={{{index * 2 + 1}}};";
                    index++;
                }
                await _lougeDbContext.Database.ExecuteSqlRawAsync(allSql, manyToOneNavigationTables.SelectMany(x => new List<object> { x.Value, x.CurrentValue }).ToArray());

            }
        }


        #endregion <方法>

        #region <事件>
        #endregion <事件>
    }
}
