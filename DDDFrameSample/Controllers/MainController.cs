using Infra.WebApi.Controllers;
using Infra.WebApi.Service;
using Microsoft.AspNetCore.Mvc;

namespace DDDFrameSample.Controllers
{
    [Route("DDDFrameSample/[controller]")]
    [ApiController]
    public class MainController : MainControllerBase
    {
        public MainController(IMainAppService mainAppServiceBase) : base(mainAppServiceBase)
        {
        }
    }
}
