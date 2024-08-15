namespace Celebratix.Common.Models.DTOs.User;

public class StripeAccountLinkResponseDto
{
    public string Url { get; set; } = null!;

    public DateTimeOffset ExpiresAt { get; set; }
}
