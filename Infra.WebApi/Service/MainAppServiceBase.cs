using Infra.Core.Abstract;
using Infra.Core.DTOs;
using Infra.Core.Extensions;
using Infra.EF.Service;
using Infra.WebApi.DTOs;
using System.Configuration;
using System.Diagnostics;
using UniversalRpc.Extensions;

namespace Infra.WebApi.Service
{
    public abstract class MainAppServiceBase:AppServiceBase,IMainAppService
    {
        private readonly IDomainServiceFactory domainServiceFactory;
        private readonly IDomainServiceContext domainServiceContext;

        public MainAppServiceBase(IDomainServiceFactory domainServiceFactory,IDomainServiceContext domainServiceContext) {
            this.domainServiceFactory = domainServiceFactory;
            this.domainServiceContext= domainServiceContext;
        }
        public virtual async Task<FrameChangeOutputDTO> CommitAsync(FrameChangeInputDTO frameChangeInput)
        {
            try
            {
                var result = new FrameChangeOutputDTO();
                var removeDomainServiceGroup = (frameChangeInput.Deletes ?? new List<Infra.Core.Abstract.IInputDTO>())
                .GroupBy(x=>domainServiceFactory.GetDomainService(x.ObjectType)).OrderBy(x => x.Key.RemovePriority);
                var insertOrUpdateDomainServiceGroup = (frameChangeInput.FrameChanges ?? new List<Infra.Core.Abstract.IInputDTO>())
                    .GroupBy(x=>domainServiceFactory.GetDomainService(x.ObjectType)).OrderBy(x => x.Key.InsertOrUpdatePriority);

                foreach (var domainService in removeDomainServiceGroup)
                {
                    result.MergeResult(await domainService.Key.RemoveAsync(domainService.ToArray()));
                }
                foreach (var domainService in insertOrUpdateDomainServiceGroup)
                {
                    result.MergeResult(await domainService.Key.InsertOrUpdateAsync(domainService.ToArray()));
                }
                await domainServiceContext.SaveAsync();
                return result;
            }
            catch
            {
                throw;
            }


        }

        public async virtual Task<IPagedList<IOutputDTO>> PageQueryAsync(IPageQueryDTO pageQueryDTO)
        {
            var domainService = domainServiceFactory.GetDomainService(pageQueryDTO.ObjectType);
            return await domainService.PageQueryAsync(pageQueryDTO);
        }

        public async Task<IEnumerable<IOutputDTO[]>> QueryAsync(params IQueryDTO[] queryDTOs)
        {
            queryDTOs.Where(x => x.Id <= 0).ForEach(x => x.Id = IdGenerater.Yitter.IdGenerater.GetNextId());
            var groups=queryDTOs.GroupBy(x=>domainServiceFactory.GetDomainService(x.ObjectType))
                .OrderBy(x=>x.Key.QueryPriority)
                .ToArray();
            var result=new List<IOutputDTO[]>();
            var dic = new Dictionary<long, IOutputDTO[]>();
            foreach (var domainService in groups) {
                var queries = domainService.ToArray();
                var queryResult = await domainService.Key.QueryAsync(queries);
                SetDic(queries, dic,queryResult);
            }
            SetResult(dic,queryDTOs,result);
            return [.. result];
        }

        private void SetResult(Dictionary<long, IOutputDTO[]> dic, IQueryDTO[] queryDTOs, List<IOutputDTO[]> result)
        {
            for(var i = 0; i < queryDTOs.Length; i++)
            {
                var queryResult = dic[queryDTOs[i].Id];
                result.Add(queryResult);
            }
        }

        private void SetDic(IQueryDTO[] queries, Dictionary<long, IOutputDTO[]> dic, IEnumerable<IOutputDTO[]> queryResult)
        {
            for(var i = 0; i < queries.Length; i++)
            {
                var id=queries[i].Id;
                dic.Add(id, queryResult.ElementAt(i));
            }
        }

        public async Task<int[]> CountAsync(params IQueryDTO[] queryDTOs)
        {
            queryDTOs.Where(x => x.Id <= 0).ForEach(x => x.Id = IdGenerater.Yitter.IdGenerater.GetNextId());
            var groups = queryDTOs.GroupBy(x => domainServiceFactory.GetDomainService(x.ObjectType))
                .OrderBy(x => x.Key.QueryPriority)
                .ToArray();
            var dic=new Dictionary<long,int>();
            var result = new List<int>();
            foreach (var domainService in groups)
            {
                var queries = domainService.ToArray();
                var counts = await domainService.Key.CountAsync(queries);
                SetDic(queries,counts,dic);
            }
            SetResult(dic,queryDTOs,result);
            return [.. result];
        }

        private static void SetResult(Dictionary<long, int> dic, IQueryDTO[] queryDTOs, List<int> result)
        {
            for(var i = 0; i < queryDTOs.Length; i++)
            {
                result.Add(dic[queryDTOs[i].Id]);
            }
        }

        private static void SetDic(IQueryDTO[] queries, int[] counts, Dictionary<long, int> dic)
        {
            for(var i = 0; i < queries.Length; i++)
            {
                dic[queries[i].Id]=counts[i];
            }
        }

        public EnumDTO[] GetEnums(string enumName)
        {
            var enumType = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x=>x.IsNotOut())
                .SelectMany(x=>x.GetExportedTypes())
                .Where(x=>x.IsEnum)
                .FirstOrDefault(x=>x.Name.EqualsIgnoreCase(enumName));
            Debug.Assert(enumType!=null);
            return enumType.GetEnumTextValueList().Select(x=>new EnumDTO
            {
                Value=(int)x.Item1,
                Label=x.Item2,
                Text=x.Item3,
            }).ToArray();
        }
    }
}
