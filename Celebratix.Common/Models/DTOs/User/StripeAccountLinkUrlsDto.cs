namespace Celebratix.Common.Models.DTOs.User;

public class StripeAccountLinkUrlsDto
{
    public string ReturnUrl { get; set; } = null!;

    public string RefreshUrl { get; set; } = null!;
}
