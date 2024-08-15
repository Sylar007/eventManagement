namespace Celebratix.Common.Configs;

public class JwtBearerConfig
{
    public string Issuer { get; set; } = null!;

    public string Audience { get; set; } = null!;

    public string Secret { get; set; } = null!;
}
