using Infra.Core.Abstract;
using Infra.Core.DTO;
using Infra.Core.Extensions;
using Infra.EF.PG.Service;
using Infra.WebApi.DTOs;
using System.Diagnostics;

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

        public async Task<IPagedList<IOutputDTO>> PageQueryAsync(IPageQueryDTO pageQueryDTO)
        {
            var domainService = domainServiceFactory.GetDomainService(pageQueryDTO.ObjectType);
            return await domainService.PageQueryAsync(pageQueryDTO);
        }

        public async Task<IOutputDTO[]> QueryAsync(params IQueryDTO[] queryDTOs)
        {
            var groups=queryDTOs.GroupBy(x=>domainServiceFactory.GetDomainService(x.ObjectType))
                .OrderBy(x=>x.Key.QueryPriority)
                .ToArray();
            var result=new List<IOutputDTO>();
            foreach (var domainService in groups) {
                var queries = domainService.ToArray();
                result.AddRange(await domainService.Key.QueryAsync(queries));
            }
            return [.. result];
        }

        public EnumDTO[] GetEnums(string enumName)
        {
            var enumType = AppDomain.CurrentDomain.GetAssemblies()
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
