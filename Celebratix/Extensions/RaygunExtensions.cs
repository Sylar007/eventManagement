using Mindscape.Raygun4Net.AspNetCore;

namespace Celebratix.Extensions
{
    internal static class RaygunExtensions
    {
        internal static IServiceCollection AddCrashReport(this IServiceCollection services,
            bool isProduction, IConfiguration configuration)
        {
            if (isProduction)
            {
                services.AddRaygun(configuration);
            }

            return services;
        }
    }
}
