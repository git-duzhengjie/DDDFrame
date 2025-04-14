using Infra.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.EF.Service
{
    /// <summary>
    /// 
    /// </summary>
    public class UpdateData
    {
        /// <summary>
        /// 更新的对象
        /// </summary>
        public EntityBase Update { get; set; }

        /// <summary>
        /// 更新的属性名
        /// </summary>
        public string[] Properties { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IDbData
    {
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        Task SaveAsync();

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task DeleteAsync(params EntityBase[] entities);


        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="updateNavigation"></param>
        /// <returns></returns>
        Task UpdateAsync(bool updateNavigation, params EntityBase[] entities);

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="entitites"></param>
        /// <param name="updateNavigation"></param>
        /// <returns></returns>
        Task AddAsync(bool updateNavigation, params EntityBase[] entitites);

        /// <summary>
        /// 更新指定属性名的对象
        /// </summary>
        /// <param name="entitiesWithProperties">带有属性名的更新对象</param>
        /// <returns></returns>
        Task UpdateAsync(params UpdateData[] entitiesWithProperties);
    }
}
