using Twilio;

namespace Celebratix.Extensions
{
    internal static class TwilioExtensions
    {
        internal static WebApplicationBuilder AddTwilio(this WebApplicationBuilder builder)
        {
            var accountSid = builder.Configuration["Twilio:Client:AccountSID"];
            var authToken = builder.Configuration["Twilio:Client:AuthToken"];
            TwilioClient.Init(accountSid, authToken);

            return builder;
        }
    }
}
