using Infra.EF.PG.Context;
using Infra.EF.PG.Service;
using Infra.WebApi.Models;
using Infra.WebApi.Service;

namespace DDDFrameSample.Domain.Services.Implementations
{
    public class DDDFrameSampleDefaultDomainService : DomainServiceBase
    {
        public DDDFrameSampleDefaultDomainService(FrameDbContext frameDbContext, EntityFactory entityFactory, IDomainServiceContext domainServiceContext, LoginUser loginUser) : base(frameDbContext, entityFactory, domainServiceContext, loginUser)
        {
        }
    }
}
