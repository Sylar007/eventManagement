using Celebratix.Common.Configs;
using Celebratix.Common.Extensions;
using Celebratix.Common.Services.HangfireSetups;
using Celebratix.Configurations;
using Celebratix.Extensions;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using SixLabors.ImageSharp.Web.DependencyInjection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var rewriteOptions = new RewriteOptions();
const string hangfireDashboardAuthenticationPolicy = "HangfireAuthPolicy";

builder.Services
    .AddControllers(options =>
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true)
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services
    .AddIdentityConfiguration()
    .AddAuthorizationPolicies(hangfireDashboardAuthenticationPolicy)
    .AddCelebratixSwagger();

builder.Services.Configure<ApiBehaviorOptions>(options
    => options.SuppressModelStateInvalidFilter = true);

builder.Services
    .AddDatabases(
        builder.Configuration.GetConnectionString("CelebratixPsqlConnection")!,
        builder.Configuration.GetConnectionString("HangfirePsqlConnection")!,
        builder.Environment.IsDevelopment())
    .AddCrashReport(builder.Environment.IsProduction(), builder.Configuration);

builder
    .AddJwtAuthentication()
    .AddTwilio()
    .AddSentry()
    .AddS3Configuration(rewriteOptions)
    .AddConfiguration();

builder.Services
    .AddHealthChecks();

builder.Services
    .AddServices()
    .AddS3ImageStorageService()
    .AddValidators()
    .AddMappings()
    .AddSendGrid(
        builder.Configuration.GetSection(nameof(SendGridConfig)).Get<SendGridConfig>()!);

var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.MapHealthChecks("/");

app.UseCelebratixDatabase(builder,
    builder.GetSetting<SeederConfiguration>());

app.UseExceptionHandler("/error");

app.UseCors(options => options
    .WithOrigins(builder.Configuration.GetSection("Cors").GetSection("Origins").Get<string[]>()!)
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());

app.UseRewriter(rewriteOptions);

app.UseImageSharp();

app.UseCelebratixSwagger();

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHangfireDashboardWithAuthorizationPolicy(hangfireDashboardAuthenticationPolicy);

OrderWorkerSetup.Setup();
MarketplaceListingWorkerSetup.Setup();
TicketTransferWorkerSetup.Setup();

app.Run();
