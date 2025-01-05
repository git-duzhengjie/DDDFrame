using DDDFrameSample.Application.Contract.DTOS.Domains;
using DDDFrameSample.Application.Contract.Enums;
using Infra.Core.Abstract;
using Infra.Core.Extensions;
using Infra.EF.PG.Entities;

namespace DDDFrameSample.Domain.Entities
{
    public class DDDFrameSampleEntity : DDDFrameSampleEntityBase
    {
        public override IInputDTO Input => this.MapTo<DDDFrameSampleEntityInputDTO>();

        public override IOutputDTO Output => this.MapTo<DDDFrameSampleEntityOutputDTO>();

        public override int ObjectType => DDDFrameSampleObjectType.DDDFrameSampleEntity.ToInt();
    }
}
