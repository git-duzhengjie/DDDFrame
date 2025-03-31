using Infra.Core.Abstract;
using UniversalRpc.Abstracts;

namespace Infra.Core.Models
{
    [Serializable]
    public class PagedList<TEntity> : IPagedList<TEntity>,IObject
    {
        public IList<TEntity> DataList { get; set; }

        public PagedList()
        {
            Count = 1;
            DataList = new List<TEntity>();
        }

        public PagedList(IQueryable<TEntity> queryable, int page, int size)
          : this()
        {
            int num = queryable.Count();
            Total = num;
            Pages = num / size;
            if (num % size > 0)
                ++Pages;
            Count = size;
            Page = page;
            AddRange(queryable.Skip((page - 1) * size).Take(size).ToList());
        }

        public IList<TEntity> ToArray()
        {
            return DataList?.ToArray();
        }

        public void AddRange(IEnumerable<TEntity> entityList)
        {
            if (entityList == null)
                return;
            if (DataList == null)
                DataList = new List<TEntity>();
            foreach (TEntity entity in entityList)
                DataList.Add(entity);
        }

        public PagedList(IList<TEntity> list, int page, int size)
          : this()
        {
            Total = list.Count();
            Pages = Total / size;
            if (Total % size > 0)
                ++Pages;
            Count = size;
            Page = page;
            AddRange(list.Skip((page - 1) * size).Take(size).ToList());
        }

        public PagedList(IEnumerable<TEntity> enumerable, int page, int size, int total)
          : this()
        {
            Total = total;
            Pages = Total / size;
            if (Total % size > 0)
                ++Pages;
            Count = size;
            Page = page;
            AddRange(enumerable);
        }

        public IPagedList<T> ConvertData<T>(IEnumerable<T> enumerable)
        {
            return new PagedList<T>(enumerable, Page, Count, Total);
        }

        public int Page { set; get; }

        public int Count { set; get; }

        public int Total { set; get; }

        public int Pages { set; get; }

        public bool HasPrev => Page > 1;

        public bool HasNext => Page < Pages;

        public string ObjectName => "PagedList";

        public int ObjectType => -1;
    }
}
