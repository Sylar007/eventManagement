using System.Text.Json.Serialization;

namespace Celebratix.Common.Configs;

public class AppConfig
{
    [JsonIgnore]
    public string AppName { get; set; } = String.Empty;
    /// <summary>
    /// The minimum version of the app that is allowed to use this version of the API
    /// </summary>
    public string MinVersion { get; set; } = null!;

    public DateTimeOffset ServerTime { get; } = DateTimeOffset.UtcNow;
}
