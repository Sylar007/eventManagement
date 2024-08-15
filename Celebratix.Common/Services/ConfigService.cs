using Celebratix.Common.Configs;
using Microsoft.Extensions.Options;

namespace Celebratix.Common.Services;

public class ConfigService
{
    private readonly AppSettings _appSettings;

    public ConfigService(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(AppSettings));
    }

    public AppConfig? GetAppConfig(string appName)
    {
        return _appSettings.GetAppConfig(appName);
    }
}
