using Infra.WebApi.Controllers;
using Infra.WebApi.Service;
using Microsoft.AspNetCore.Mvc;

namespace DDDMicroServiceArchitectureSample.Controllers
{
    [Route("DDDMicroServiceArchitectureSample/[controller]")]
    [ApiController]
    public class MainController : MainControllerBase
    {
        public MainController(IMainAppService mainAppServiceBase) : base(mainAppServiceBase)
        {
        }
    }
}
