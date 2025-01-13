using Infra.Core.Abstract;
using Infra.Core.DTOs;
using Infra.Core.Models;
using Infra.WebApi.DTOs;
using Infra.WebApi.Service;
using Microsoft.AspNetCore.Http;
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
        /// <summary>
        /// 数据提交
        /// </summary>
        /// <param name="frameChangeInput"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CommitAsync([FromBody] FrameChangeInputDTO frameChangeInput)
        {
            var result = await mainAppService.CommitAsync(frameChangeInput);
            return Ok(result);
        }

        /// <summary>
        /// 数据查询
        /// </summary>
        /// <param name="queries"></param>
        /// <returns></returns>
        [HttpPost("query")]
        public async Task<IActionResult> QueryAsync([FromBody] IQueryDTO[] queries)
        {
            var result = await mainAppService.QueryAsync(queries);
            return Ok(result);
        }

        /// <summary>
        /// 数据统计查询
        /// </summary>
        /// <param name="queries"></param>
        /// <returns></returns>
        [HttpPost("count")]
        public async Task<IActionResult> CountAsync([FromBody] IQueryDTO[] queries)
        {
            var result = await mainAppService.CountAsync(queries);
            return Ok(result);
        }

        /// <summary>
        /// 数据分页查询
        /// </summary>
        /// <param name="pageQuery"></param>
        /// <returns></returns>
        [HttpPost("page")]
        public async Task<IActionResult> PageQueryAsync([FromBody] IPageQueryDTO pageQuery)
        {
            var result = await mainAppService.PageQueryAsync(pageQuery);
            return Ok(result);
        }
    }
}
