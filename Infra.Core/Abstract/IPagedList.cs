using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Core.Abstract
{
    public interface IPagedList<TEntity>
    {
        /// <summary>
        /// 数据
        /// </summary>
        IList<TEntity> DataList { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        int Page { get; }

        /// <summary>
        /// 每页个数
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 总数
        /// </summary>
        int Total { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        int Pages { get; set; }

        bool HasPrev { get; }

        bool HasNext { get; }

        IPagedList<T> ConvertData<T>(IEnumerable<T> enumerable);

        IList<TEntity> ToArray();

        void AddRange(IEnumerable<TEntity> entityList);
    }
}
