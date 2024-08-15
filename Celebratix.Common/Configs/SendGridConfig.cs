namespace Celebratix.Common.Configs;

public class SendGridConfig
{
    public string ApiKey { get; set; } = null!;
    public string SendFrom { get; set; } = null!;
    public string SenderName { get; set; } = null!;
    public string ForgotPasswordId { get; set; } = null!;
    public string EmailConfirmationId { get; set; } = null!;
    public string AdminAccountSetupId { get; set; } = null!;
    public string BusinessAccountSetupId { get; set; } = null!;
    public string TicketPurchaseCompleteId { get; set; } = null!;
}
