using DDDFrameSample.Application.Contract.Enums;
using Infra.Core.Abstract;
using Infra.Core.Extensions;

namespace DDDFrameSample.Application.Contract.DTOS.Domains
{
    public class DDDFrameSampleEntityOutputDTO : DDDFrameSampleEntityBaseOutputDTO
    {
        public override int ObjectType => DDDFrameSampleObjectType.DDDFrameSampleEntity.ToInt();

        public override IInputDTO GetInput()
        {
            return this.MapTo<DDDFrameSampleEntityInputDTO>();
        }
    }
}
