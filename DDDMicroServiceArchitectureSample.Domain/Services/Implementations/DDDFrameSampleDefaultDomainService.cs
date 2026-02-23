using Infra.EF.Context;
using Infra.EF.Service;
using Infra.WebApi.Service;

namespace DDDMicroServiceArchitectureSample.Domain.Services.Implementations
{
    public class DDDMicroServiceArchitectureSampleDefaultDomainService : DomainServiceBase
    {
        public DDDMicroServiceArchitectureSampleDefaultDomainService(FrameDbContextBase frameDbContext, EntityFactory entityFactory, IDomainServiceContext domainServiceContext) : base(frameDbContext, entityFactory, domainServiceContext)
        {
        }
    }
}
