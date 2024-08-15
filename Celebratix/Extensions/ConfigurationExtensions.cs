using Celebratix.Common.Configs;
using Stripe;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Celebratix.Extensions
{
    internal static class ConfigurationExtensions
    {
        internal static void AddConfiguration(this WebApplicationBuilder builder)
        {
            builder.Services
                .Configure<JwtBearerConfig>(builder.Configuration.GetSection(nameof(JwtBearerConfig)))
                .Configure<SendGridConfig>(builder.Configuration.GetSection(nameof(SendGridConfig)))
                .Configure<UrlConfig>(builder.Configuration.GetSection(nameof(UrlConfig)))
                .Configure<AwsS3Config>(builder.Configuration.GetSection(nameof(AwsS3Config)))
                .Configure<StripeConfig>(builder.Configuration.GetSection(nameof(StripeConfig)))
                .Configure<MarketplaceConfig>(builder.Configuration.GetSection(nameof(MarketplaceConfig)))
                .Configure<TicketScanningConfig>(builder.Configuration.GetSection(nameof(TicketScanningConfig)))
                .Configure<TwilioVerifySettings>(builder.Configuration.GetSection("Twilio"))
                .AddOptions<AppSettings>(builder.Configuration);

            StripeConfiguration.ApiKey =
                builder.Configuration.GetSection(nameof(StripeConfig)).Get<StripeConfig>()!.ApiKey;
        }

        private static void AddOptions<T>(this IServiceCollection services,
            IConfiguration configuration) where T : class =>
            services
                .AddOptions<T>()
                .Bind(configuration.GetSection(typeof(T).Name));

        internal static T? GetSetting<T>(this WebApplicationBuilder builder)
            where T : class =>
            builder.Configuration.GetSection(typeof(T).Name).Get<T>();
    }

    [ExcludeFromCodeCoverage]
    [Serializable]
    public class ServiceConfigurationException : Exception
    {
        public ServiceConfigurationException()
        { }

        public ServiceConfigurationException(string message, Exception inner) : base(message, inner)
        { }

        public ServiceConfigurationException(string validationErrors) : base(validationErrors)
        { }

        protected ServiceConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
