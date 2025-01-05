using DDDFrameSample.Domain.Services.Implementations;
using Infra.WebApi.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDDFrameSample.Domain.Services
{
    public class DomainServiceFactory : IDomainServiceFactory
    {
        private DDDFrameSampleDefaultDomainService dDDFrameSampleDefaultDomainService;

        public DomainServiceFactory(DDDFrameSampleDefaultDomainService dDDFrameSampleDefaultDomainService) {
            this.dDDFrameSampleDefaultDomainService=dDDFrameSampleDefaultDomainService;
        }
        public IDomainService GetDomainService(int objectType)
        {
            return dDDFrameSampleDefaultDomainService;
        }
    }
}
