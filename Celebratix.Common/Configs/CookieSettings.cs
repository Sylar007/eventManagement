namespace Celebratix.Settings
{
    public class CookieSettings
    {
        public string SameSite { get; set; } = string.Empty;
        public int SignatureExpirationTimeInMinutes { get; set; }
    }
}
