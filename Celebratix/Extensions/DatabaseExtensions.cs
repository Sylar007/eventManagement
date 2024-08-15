using Celebratix.Common.Database;
using Celebratix.Common.Models;
using Celebratix.Configurations;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Celebratix.Extensions;

[ExcludeFromCodeCoverage]
internal static class DatabaseExtensions
{
    internal static IServiceCollection AddDatabases(this IServiceCollection services,
        string celebratixConnectionString,
        string hangFireConnectionString, bool isDevelopment)
    {
        services
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddDbContext<CelebratixDbContext>(
                options =>
                {
                    options.UseNpgsql(celebratixConnectionString,
                        b => b.MigrationsAssembly("Celebratix"));
                    options.EnableSensitiveDataLogging();
                })
            .AddHangfireService(hangFireConnectionString, isDevelopment);

        return services;
    }

    internal static void UseCelebratixDatabase(this WebApplication app,
        WebApplicationBuilder builder, SeederConfiguration? seederConfiguration)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CelebratixDbContext>();

        dbContext.Database.Migrate();

        if (seederConfiguration == null || seederConfiguration.Enabled)
        {
            CelebratixDbContext.SeedCustomData(
                app.Services,
                builder.Environment.IsDevelopment());
        }
    }
}
