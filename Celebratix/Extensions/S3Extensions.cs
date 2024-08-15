using Celebratix.Common.Configs;
using Celebratix.Common.Services;
using Celebratix.Common.Services.Interfaces;
using Microsoft.AspNetCore.Rewrite;
using SixLabors.ImageSharp.Web.Caching.AWS;
using SixLabors.ImageSharp.Web.DependencyInjection;
using SixLabors.ImageSharp.Web.Providers.AWS;

namespace Celebratix.Extensions
{
    internal static class S3Extensions
    {
        internal static IServiceCollection AddS3ImageStorageService(this IServiceCollection services) =>
            services
                .AddScoped(typeof(S3StorageService))
                .AddScoped<IImageStorageService, S3ImageStorageService>();

        internal static WebApplicationBuilder AddS3Configuration(this WebApplicationBuilder builder, RewriteOptions rewriteOptions)
        {
            var awsS3Config = builder.Configuration.GetSection(nameof(AwsS3Config)).Get<AwsS3Config>()!;
            var serviceUrl = $"https://{awsS3Config.Endpoint}";
            builder.Services.AddImageSharp(options =>
                {
                    options.BrowserMaxAge = TimeSpan.FromDays(7);
                    options.CacheMaxAge = TimeSpan.FromDays(365);
                })
                .ClearProviders()
                .Configure<AWSS3StorageImageProviderOptions>(options =>
                {
                    options.S3Buckets.Add(new AWSS3BucketClientOptions
                    {
                        AccessKey = awsS3Config.AccessKey,
                        AccessSecret = awsS3Config.AccessSecret,
                        BucketName = awsS3Config.FileBucketName,
                        Endpoint = serviceUrl,
                        Region = awsS3Config.Region
                    });
                })
                .Configure<AWSS3StorageCacheOptions>(options =>
                {
                    options.AccessKey = awsS3Config.AccessKey;
                    options.AccessSecret = awsS3Config.AccessSecret;
                    options.BucketName = awsS3Config.ImageCacheBucketName;
                    options.Endpoint = serviceUrl;
                    options.Region = awsS3Config.Region;
                })
                .AddProvider<AWSS3StorageImageProvider>()
                .SetCache<AWSS3StorageCache>();

            rewriteOptions.AddRewrite("^files/(.*)", $"{awsS3Config.FileBucketName}/{awsS3Config.ImageDirectory}/$1", skipRemainingRules: false);

            return builder;
        }
    }
}
