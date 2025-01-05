using DDDFrameSample.Application.Contract.Enums;
using Infra.Core.Abstract;
using Infra.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDDFrameSample.Application.Contract.DTOS.Domains
{
    public class DDDFrameSampleEntityInputDTO : DDDFrameSampleEntityBaseInputDTO
    {
        public override int ObjectType => DDDFrameSampleObjectType.DDDFrameSampleEntity.ToInt();
    }
}
