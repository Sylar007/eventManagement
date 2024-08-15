using Celebratix.Common.Configs;
using Celebratix.Settings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System.Text;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

namespace Celebratix.Extensions;

internal static class AuthenticationExtensions
{
    internal static WebApplicationBuilder AddJwtAuthentication(this WebApplicationBuilder builder)
    {
        var jwtBearerConfig = builder.Configuration.GetSection(nameof(JwtBearerConfig)).Get<JwtBearerConfig>()!;
        var cookieSettings = builder.Configuration.GetSection(nameof(CookieSettings)).Get<CookieSettings>()!;

        builder.Services
            .AddAuthentication(options =>
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(cookieSettings.SignatureExpirationTimeInMinutes);
                options.SlidingExpiration = true;
                // Used to prevent CSRF, requires that the frontend is on the same domain!
                // https://www.sjoerdlangkemper.nl/2016/04/14/preventing-csrf-with-samesite-cookie-attribute/
                // For localhost specifically SameSiteMode.None seems problematic on chrome & edge
                options.Cookie.SameSite = cookieSettings.SameSite == "strict"
                    ? SameSiteMode.Strict
                    : SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.LoginPath = null;
                options.LogoutPath = null;
                options.AccessDeniedPath = null;
            })
            .AddJwtBearer(options =>
            {
                var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtBearerConfig.Secret));
                options.SaveToken = true;
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = jwtBearerConfig.Audience,
                    ValidIssuer = jwtBearerConfig.Issuer,
                    IssuerSigningKey = signingKey
                };

                options.ForwardDefaultSelector = context =>
                {
                    string authorization = context.Request.Headers[HeaderNames.Authorization]!;
                    string cookie = context.Request.Headers[HeaderNames.Cookie]!;

                    if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
                        return JwtBearerDefaults.AuthenticationScheme;

                    if (!string.IsNullOrEmpty(cookie) && cookie.StartsWith("Cookies"))
                        return CookieAuthenticationDefaults.AuthenticationScheme;

                    return IdentityConstants.ApplicationScheme;
                };
            });

        return builder;
    }
}