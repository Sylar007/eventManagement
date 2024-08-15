using Celebratix.Common.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Celebratix.Extensions
{
    internal static class AuthorizationExtensions
    {
        internal static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services,
            string hangfireDashboardAuthenticationPolicy) =>
            services.AddAuthorization(options =>
            {
                // Policy to be applied to hangfire endpoint
                options.AddPolicy(hangfireDashboardAuthenticationPolicy, policyBuilder =>
                {
                    policyBuilder.RequireAuthenticatedUser();
                    policyBuilder.RequireRole(Enums.SuperAdmin);
                });


                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    JwtBearerDefaults.AuthenticationScheme
                );
                defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            });

    }
}
