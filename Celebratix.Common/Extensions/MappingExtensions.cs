using Celebratix.Common.Mappings;
using Microsoft.Extensions.DependencyInjection;

namespace Celebratix.Common.Extensions
{
    public static class MappingExtensions
    {
        public static IServiceCollection AddMappings(this IServiceCollection services) =>
            services.AddAutoMapper(typeof(MappingProfile));
    }
}