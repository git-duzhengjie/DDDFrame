using DDDMicroServiceArchitectureSample.Application.Contract.DTOS.Domains;
using DDDMicroServiceArchitectureSample.Application.Contract.Enums;
using Infra.Core.Abstract;
using Infra.Core.Extensions;
using Infra.EF.Entities;

namespace DDDMicroServiceArchitectureSample.Domain.Entities
{
    public class DDDMicroServiceArchitectureSampleEntity : DDDMicroServiceArchitectureSampleEntityBase
    {
        public override IInputDTO Input => this.MapTo<DDDMicroServiceArchitectureSampleEntityInputDTO>();

        public override IOutputDTO Output => this.MapTo<DDDMicroServiceArchitectureSampleEntityOutputDTO>();

        public override int ObjectType => DDDMicroServiceArchitectureSampleObjectType.DDDMicroServiceArchitectureSampleEntity.ToInt();
    }
}
