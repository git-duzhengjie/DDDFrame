using Infra.WebApi.DTOs;
using Infra.WebApi.Service;
using Microsoft.AspNetCore.Mvc;

namespace Infra.WebApi.Controllers
{
    public abstract class MainControllerBase:FrameControllerBase
    {
        private IMainAppService mainAppService;

        public MainControllerBase(IMainAppService mainAppServiceBase) { 
            this.mainAppService=mainAppServiceBase;
        }
        /// <summary>
        /// 查询枚举值
        /// </summary>
        /// <param name="enumName">枚举名</param>
        /// <returns></returns>
        [HttpGet("enum")]
        public EnumDTO[] GetEnums(string enumName) {
            return mainAppService.GetEnums(enumName);
        }

    }
}
