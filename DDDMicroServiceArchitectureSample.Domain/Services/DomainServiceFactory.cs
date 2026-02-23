using DDDMicroServiceArchitectureSample.Domain.Services.Implementations;
using Infra.WebApi.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDDMicroServiceArchitectureSample.Domain.Services
{
    public class DomainServiceFactory : IDomainServiceFactory
    {
        private DDDMicroServiceArchitectureSampleDefaultDomainService DDDMicroServiceArchitectureSampleDefaultDomainService;

        public DomainServiceFactory(DDDMicroServiceArchitectureSampleDefaultDomainService DDDMicroServiceArchitectureSampleDefaultDomainService) {
            this.DDDMicroServiceArchitectureSampleDefaultDomainService=DDDMicroServiceArchitectureSampleDefaultDomainService;
        }
        public IDomainService GetDomainService(int objectType)
        {
            return DDDMicroServiceArchitectureSampleDefaultDomainService;
        }
    }
}
