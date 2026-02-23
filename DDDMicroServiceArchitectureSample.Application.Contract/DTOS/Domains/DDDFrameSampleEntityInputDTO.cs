using DDDMicroServiceArchitectureSample.Application.Contract.Enums;
using Infra.Core.Abstract;
using Infra.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDDMicroServiceArchitectureSample.Application.Contract.DTOS.Domains
{
    public class DDDMicroServiceArchitectureSampleEntityInputDTO : DDDMicroServiceArchitectureSampleEntityBaseInputDTO
    {
        public override int ObjectType => DDDMicroServiceArchitectureSampleObjectType.DDDMicroServiceArchitectureSampleEntity.ToInt();
    }
}
