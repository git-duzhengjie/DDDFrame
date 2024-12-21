using Infra.Core.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Core.DTO
{
    public class FrameChangeInputDTO
    {
        /// <summary>
        /// 新增或者更新对象
        /// </summary>
        public List<IInputDTO> FrameChanges { get; set; }

        /// <summary>
        /// 删除对象
        /// </summary>
        public List<IInputDTO> Deletes { get; set; }
    }
}
