using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Celebratix.Common.SwaggerFilters;
using Celebratix.Configurations;
using Celebratix.Filters;

namespace Celebratix.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddCelebratixSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SchemaGeneratorOptions.SupportNonNullableReferenceTypes = true;
                options.SchemaFilter<SetAllRequiredSchemaFilter>();
                options.OperationFilter<SwaggerDefaultValuesFilter>();
            });

            services
                .ConfigureOptions<ConfigureSwaggerOptions>()
                .AddApiVersioning(o =>
                {
                    o.DefaultApiVersion = new ApiVersion(1, 0);
                    o.AssumeDefaultVersionWhenUnspecified = true;
                    o.ReportApiVersions = true;
                })
                .AddApiExplorer(o =>
                {
                    o.GroupNameFormat = "'v'VVV";
                    o.SubstituteApiVersionInUrl = true;
                });

            return services;
        }

        public static void UseCelebratixSwagger(this IApplicationBuilder application)
        {
            application.UseSwagger();

            application.UseSwaggerUI(options =>
            {
                var apiVersionProvider = application.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var groupName in apiVersionProvider.ApiVersionDescriptions.Select(d => d.GroupName))
                {
                    options.SwaggerEndpoint($"{groupName}/swagger.json", groupName.ToLowerInvariant());
                }
            });
        }
    }
}