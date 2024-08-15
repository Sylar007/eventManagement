namespace Celebratix.Common.Configs;

public class AppSettings
{
    public AppConfig[] AppConfigs { get; set; } = { };

    public AppConfig? GetAppConfig(string appName) =>
        AppConfigs.FirstOrDefault(config => config.AppName == appName);
}