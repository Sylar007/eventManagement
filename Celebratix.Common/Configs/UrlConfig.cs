namespace Celebratix.Common.Configs
{
    public class UrlConfig
    {
        public const string UserIdIdentifier = "{userId}";
        public const string TokenIdentifier = "{token}";
        public const string EmailIdentifier = "{email}";
        public const string LinkIdentifier = "{link}";
        public const string OrderIdIdentifier = "{orderId}";
        public const string MagicIdentifier = "{magic}";

        public string AdminBaseUrl { get; set; } = null!;

        public string BusinessBaseUrl { get; set; } = null!;

        public string AppBaseUrl { get; set; } = null!;

        public string OrderRelativeRef { get; set; } = null!;
        public string EmailConfirmationRelativeRef { get; set; } = null!;
        public string ActivateAccountRelativeRef { get; set; } = null!;
        public string PasswordResetRelativeRef { get; set; } = null!;
        public string MagicOrderRelativeRef { get; set; } = null!;
    }
}
