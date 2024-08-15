using Celebratix.Common.Models;
using Celebratix.Common.Models.DbModels;
using Microsoft.AspNetCore.Identity;

namespace Celebratix.Extensions
{
    internal static class IdentityExtensions
    {
        internal static IServiceCollection AddIdentityConfiguration(this IServiceCollection services)
        {
            services
                .AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<CelebratixDbContext>()
                .AddDefaultTokenProviders();

            return services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // Sign in settings
                options.SignIn.RequireConfirmedEmail = false;

                // General settings
                options.User.RequireUniqueEmail = false;
            });
        }
    }
}
