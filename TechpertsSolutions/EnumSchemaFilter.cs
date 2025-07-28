using Core.Utilities;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TechpertsSolutions
{
    public class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                var enumValues = Enum.GetValues(context.Type).Cast<Enum>();
                schema.Enum.Clear();
                foreach (var enumVal in enumValues)
                {
                    var stringValue = enumVal.GetStringValue(); 
                    schema.Enum.Add(new OpenApiString(stringValue));
                }
                schema.Type = "string";
            }
        }
    }
}
