using DDDFrameSample.Application.Services.Abstractions;
using Infra.EF.PG.Service;
using Infra.WebApi.Service;

namespace DDDFrameSample.Application.Services.Implementations
{
    public class MainAppService : MainAppServiceBase, IDDDFrameSampleService
    {
        public MainAppService(IDomainServiceFactory domainServiceFactory, IDomainServiceContext domainServiceContext) : base(domainServiceFactory, domainServiceContext)
        {
        }
    }
}
