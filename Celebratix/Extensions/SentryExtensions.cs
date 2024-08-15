using Celebratix.Common.Configs;
namespace Celebratix.Extensions
{
    internal static class SentryExtensions
    {
        internal static WebApplicationBuilder AddSentry(this WebApplicationBuilder builder)
        {
            if (!builder.Environment.IsProduction())
                return builder;

            var imageRootPath = $"/{builder.Configuration.GetSection(nameof(AwsS3Config)).Get<AwsS3Config>()!.FileBucketName}";
            builder.WebHost.UseSentry(options =>
            {
                options.SetBeforeSendTransaction((sentryEvent, hint) =>
                {
                    if (sentryEvent.Request.Url == null)
                        return sentryEvent;

                    var uri = new Uri(sentryEvent.Request.Url);
                    if (uri.AbsolutePath.StartsWith(imageRootPath))
                        return null;

                    return sentryEvent;
                });
            });
            return builder;
        }
    }
}
