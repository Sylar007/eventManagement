using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Celebratix.Common.SwaggerFilters;

public class SetAllRequiredSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Properties == null)
        {
            return;
        }

        var optionalProps = context.Type.GetProperties().Where(t => t.GetCustomAttribute<SwaggerOptionalAttribute>() != null).ToList();

        foreach (var property in schema.Properties)
        {
            if (!schema.Required.Contains(property.Key)
                && !optionalProps.Any(op => string.Equals(op.Name, property.Key, StringComparison.InvariantCultureIgnoreCase)))
            {
                schema.Required.Add(property.Key);
            }
        }
    }

}
