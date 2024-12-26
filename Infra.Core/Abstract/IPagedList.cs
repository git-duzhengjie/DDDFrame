using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Core.Abstract
{
    public interface IPagedList<TEntity>
    {
        IList<TEntity> DataList { get; set; }

        int Page { get; }

        int Count { get; }

        int Total { get; }

        int Pages { get; }

        bool HasPrev { get; }

        bool HasNext { get; }

        IPagedList<T> ConvertData<T>(IEnumerable<T> enumerable);

        IList<TEntity> ToArray();

        void AddRange(IEnumerable<TEntity> entityList);
    }
}
