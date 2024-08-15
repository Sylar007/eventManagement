#pragma warning disable CS8618
namespace Celebratix.Common.Configs;

public class StripeConfig
{
    public string PublicClientKey { get; set; }

    public string ApiKey { get; set; }

    public string AccountWebhookSecret { get; set; }

    public string ConnectWebhookSecret { get; set; }

    public string PaymentReturnUrl { get; set; }

    public int OrderTimeoutMinutes { get; set; } = 25;
}
