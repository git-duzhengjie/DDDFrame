using Infra.EF.Context;
using Infra.EF.Service;
using Microsoft.Extensions.Logging;

namespace DDDMicroServiceArchitectureSample.Domain.Services
{
    public class DDDMicroServiceArchitectureSampleContext : DomainServiceContextBase
    {
        public DDDMicroServiceArchitectureSampleContext(FrameDbContextBase frameDbContext, IDbData dbData, ILoggerFactory loggerFactory) : base(frameDbContext, dbData, loggerFactory)
        {
        }
    }
}
