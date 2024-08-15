using Celebratix.Common.Configs;
using Celebratix.Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace Celebratix.Controllers;


[Route("[controller]")]
[ApiController]
public class ConfigController : ControllerBase
{
    private readonly ConfigService _configService;

    public ConfigController(ConfigService configService)
    {
        _configService = configService;
    }

    [HttpGet("b2c-app")]
    public ActionResult<AppConfig?> GetB2CAppVersion()
    {
        return _configService.GetAppConfig("b2c-app");
    }

    [HttpGet("b2b-app")]
    public ActionResult<AppConfig?> GetB2BAppVersion()
    {
        return _configService.GetAppConfig("b2b-app");
    }
}
