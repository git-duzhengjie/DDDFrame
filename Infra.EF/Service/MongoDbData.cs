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
                var toArrayMethod = this.GetMethod("ToArrayType");
                toArrayMethod=toArrayMethod.MakeGenericMethod(gr.Key);
                await (method.Invoke(dbContext, [toArrayMethod.Invoke(null, [gr])]) as Task);
            }
        }

        public static T[] ToArrayType<T>(IGrouping<Type, EntityBase> gr)
        {
            return [.. gr.OfType<T>()];
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
                var toArrayMethod = this.GetMethod("ToArrayType");
                toArrayMethod = toArrayMethod.MakeGenericMethod(gr.Key);
                await (method.Invoke(dbContext, [toArrayMethod.Invoke(null, [gr])]) as Task);
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
