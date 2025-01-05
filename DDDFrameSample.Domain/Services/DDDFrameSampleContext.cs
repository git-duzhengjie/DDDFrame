using Infra.EF.PG.Context;
using Infra.EF.PG.Service;
using Microsoft.Extensions.Logging;

namespace DDDFrameSample.Domain.Services
{
    public class DDDFrameSampleContext : DomainServiceContextBase
    {
        public DDDFrameSampleContext(FrameDbContext frameDbContext, IDbData dbData, ILoggerFactory loggerFactory) : base(frameDbContext, dbData, loggerFactory)
        {
        }
    }
}
