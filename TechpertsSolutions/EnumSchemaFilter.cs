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
                schema.Type = "string";
                
                foreach (var enumVal in enumValues)
                {
                    var stringValue = enumVal.GetStringValue(); 
                    schema.Enum.Add(new OpenApiString(stringValue));
                }
            }
            
            // Handle nullable enums
            if (context.Type.IsGenericType && 
                context.Type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                context.Type.GetGenericArguments()[0].IsEnum)
            {
                var enumType = context.Type.GetGenericArguments()[0];
                var enumValues = Enum.GetValues(enumType).Cast<Enum>();
                schema.Enum.Clear();
                schema.Type = "string";
                schema.Nullable = true;
                
                foreach (var enumVal in enumValues)
                {
                    var stringValue = enumVal.GetStringValue(); 
                    schema.Enum.Add(new OpenApiString(stringValue));
                }
            }
        }
    }
}
