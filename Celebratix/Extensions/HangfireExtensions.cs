using Hangfire;
using Hangfire.PostgreSql;
using Newtonsoft.Json;

namespace Celebratix.Extensions;

internal static class HangfireExtensions
{
    internal static void AddHangfireService(this IServiceCollection services,
        string connectionString, bool isDevelopment) =>
        services
            .AddHangfire(configuration =>
            {
                configuration
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseSerializerSettings(new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto,
                        DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                        NullValueHandling = NullValueHandling.Ignore,
                        CheckAdditionalContent = true,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });

                if (isDevelopment)
                {
                    configuration
                        .UseInMemoryStorage()
                        .WithJobExpirationTimeout(TimeSpan.FromDays(100));
                }
                else
                {
                    configuration
                        .UsePostgreSqlStorage(storage =>
                            storage.UseNpgsqlConnection(connectionString))
                        .WithJobExpirationTimeout(TimeSpan.FromDays(100));
                }
            })
            .AddHangfireServer();
}
