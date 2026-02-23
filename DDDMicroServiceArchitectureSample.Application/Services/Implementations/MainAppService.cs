using DDDMicroServiceArchitectureSample.Application.Services.Abstractions;
using Infra.EF.Service;
using Infra.WebApi.Service;

namespace DDDMicroServiceArchitectureSample.Application.Services.Implementations
{
    public class MainAppService : MainAppServiceBase, IDDDMicroServiceArchitectureSampleService
    {
        public MainAppService(IDomainServiceFactory domainServiceFactory, IDomainServiceContext domainServiceContext) : base(domainServiceFactory, domainServiceContext)
        {
        }
    }
}
