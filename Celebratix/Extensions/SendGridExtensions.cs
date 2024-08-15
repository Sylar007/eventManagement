using Celebratix.Common.Configs;
using SendGrid;

namespace Celebratix.Extensions
{
    internal static class SendGridExtensions
    {
        public static IServiceCollection AddSendGrid(this IServiceCollection services,
            SendGridConfig sendGridConfig) =>
            services.AddScoped<ISendGridClient>(_ =>
                new SendGridClient(sendGridConfig.ApiKey));
    }
}
