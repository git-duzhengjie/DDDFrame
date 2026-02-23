using DDDMicroServiceArchitectureSample.Application.Contract.Enums;
using Infra.Core.Abstract;
using Infra.Core.Extensions;

namespace DDDMicroServiceArchitectureSample.Application.Contract.DTOS.Domains
{
    public class DDDMicroServiceArchitectureSampleEntityOutputDTO : DDDMicroServiceArchitectureSampleEntityBaseOutputDTO
    {
        public override int ObjectType => DDDMicroServiceArchitectureSampleObjectType.DDDMicroServiceArchitectureSampleEntity.ToInt();

        public override IInputDTO GetInput()
        {
            return this.MapTo<DDDMicroServiceArchitectureSampleEntityInputDTO>();
        }
    }
}
