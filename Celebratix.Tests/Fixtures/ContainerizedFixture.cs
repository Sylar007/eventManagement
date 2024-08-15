using Celebratix.Tests.Authentication;
using Celebratix.Tests.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;

namespace Celebratix.Tests.Fixtures;

public sealed class ContainerizedFixture : WebApplicationFactory<TestPlaceholder>,
    IAsyncLifetime
{
    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        builder.ConfigureServices(services =>
            {
                services.SetupJwtBearerOptions();
            })
            .WithConnectionString(_postgreSqlContainer.GetConnectionString())
            .WithoutDataSeeder();

    private readonly PostgreSqlContainer _postgreSqlContainer =
        new PostgreSqlBuilder().Build();

    public Task InitializeAsync() =>
        _postgreSqlContainer.StartAsync();

    public new Task DisposeAsync() =>
        _postgreSqlContainer.DisposeAsync().AsTask();
}