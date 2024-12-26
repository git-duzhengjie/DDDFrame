using Infra.Core.Abstract;

namespace Infra.WebApi.Service
{
    public interface IDomainServiceFactory:IFactory
    {
        public IDomainService GetDomainService(int objectType);
    }
}
