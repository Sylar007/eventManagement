using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Celebratix.Tests.Authentication;

public static class AuthenticationExtensions
{
    public static IServiceCollection SetupJwtBearerOptions(this IServiceCollection services)
    {
        services.Configure<JwtBearerOptions>(
            JwtBearerDefaults.AuthenticationScheme,
            options =>
            {
                options.Configuration = CreateOpenIdConfiguration();
                options.SecurityTokenValidators.Clear();
                options.SecurityTokenValidators.Add(new JwtSecurityTokenHandler());
                options.TokenValidationParameters =
                    SetTokenValidationParameters(LocalJwtToken.Audience);
            });

        return services;
    }

    private static TokenValidationParameters SetTokenValidationParameters(
        string audience) =>
        new()
        {
            ValidAudience = audience
        };

    private static OpenIdConnectConfiguration CreateOpenIdConfiguration() =>
        new()
        {
            Issuer = LocalJwtToken.Issuer,
            SigningKeys = { LocalJwtToken.SecurityKey }
        };
}