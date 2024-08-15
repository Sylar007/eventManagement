using Microsoft.AspNetCore.Hosting;

namespace Celebratix.Tests.Extensions;

public static class WebHostBuilderExtensions
{
    public static IWebHostBuilder WithoutConnectionString(this IWebHostBuilder builder) =>
        builder.UseSetting("ConnectionStrings:CelebratixPsqlConnection", string.Empty);

    public static IWebHostBuilder WithConnectionString(this IWebHostBuilder builder,
        string connectionString) =>
        builder.UseSetting("ConnectionStrings:CelebratixPsqlConnection", connectionString);

    public static IWebHostBuilder WithoutDataSeeder(this IWebHostBuilder builder) =>
        builder.UseSetting("SeederConfiguration:Enabled", "false");

}