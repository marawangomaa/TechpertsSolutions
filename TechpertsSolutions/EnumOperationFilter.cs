using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Core.Enums;
using Core.Utilities;
using System.Reflection;

namespace TechpertsSolutions
{
    public class EnumOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            foreach (var parameter in operation.Parameters)
            {
                var parameterInfo = context.ApiDescription.ParameterDescriptions
                    .FirstOrDefault(p => p.Name == parameter.Name);

                if (parameterInfo?.ModelMetadata?.ModelType != null)
                {
                    var parameterType = parameterInfo.ModelMetadata.ModelType;
                    
                    if (parameterType.IsEnum)
                    {
                        parameter.Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Enum = Enum.GetValues(parameterType)
                                .Cast<object>()
                                .Select(e => new OpenApiString(((Enum)e).GetStringValue()))
                                .Cast<IOpenApiAny>()
                                .ToList()
                        };
                    }
                }
            }

            // Handle request body parameters
            if (operation.RequestBody?.Content != null)
            {
                foreach (var content in operation.RequestBody.Content.Values)
                {
                    if (content.Schema?.Properties != null)
                    {
                        foreach (var property in content.Schema.Properties)
                        {
                            var propertyType = context.ApiDescription.ParameterDescriptions
                                .FirstOrDefault(p => p.Name == property.Key)?.ModelMetadata?.ModelType;

                            if (propertyType?.IsEnum == true)
                            {
                                property.Value.Type = "string";
                                property.Value.Enum = Enum.GetValues(propertyType)
                                    .Cast<object>()
                                    .Select(e => new OpenApiString(((Enum)e).GetStringValue()))
                                    .Cast<IOpenApiAny>()
                                    .ToList();
                            }
                        }
                    }
                }
            }
        }
    }
} 