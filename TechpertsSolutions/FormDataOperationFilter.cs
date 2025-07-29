using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Core.Enums;
using Core.Utilities;

namespace TechpertsSolutions
{
    public class FormDataOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var methodInfo = context.MethodInfo;
            var parameters = methodInfo.GetParameters();

            foreach (var parameter in parameters)
            {
                if (parameter.GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.FromFormAttribute), false).Any())
                {
                    
                    operation.RequestBody = null;

                    
                    if (operation.Parameters == null)
                        operation.Parameters = new List<OpenApiParameter>();

                    
                    if (parameter.ParameterType.Name == "CategoryCreateDTO")
                    {
                        operation.Parameters.Add(new OpenApiParameter
                        {
                            Name = "name",
                            In = ParameterLocation.Query,
                            Required = true,
                            Schema = new OpenApiSchema { Type = "string" },
                            Description = "Category name"
                        });

                        operation.Parameters.Add(new OpenApiParameter
                        {
                            Name = "description",
                            In = ParameterLocation.Query,
                            Required = false,
                            Schema = new OpenApiSchema { Type = "string" },
                            Description = "Category description"
                        });

                        operation.Parameters.Add(new OpenApiParameter
                        {
                            Name = "image",
                            In = ParameterLocation.Query,
                            Required = false,
                            Schema = new OpenApiSchema { Type = "string" },
                            Description = "Category image URL"
                        });
                    }

                    
                    if (parameter.ParameterType.Name == "CategoryUpdateDTO")
                    {
                        operation.Parameters.Add(new OpenApiParameter
                        {
                            Name = "id",
                            In = ParameterLocation.Query,
                            Required = true,
                            Schema = new OpenApiSchema { Type = "string" },
                            Description = "Category ID"
                        });

                        operation.Parameters.Add(new OpenApiParameter
                        {
                            Name = "name",
                            In = ParameterLocation.Query,
                            Required = true,
                            Schema = new OpenApiSchema { Type = "string" },
                            Description = "Category name"
                        });

                        operation.Parameters.Add(new OpenApiParameter
                        {
                            Name = "description",
                            In = ParameterLocation.Query,
                            Required = false,
                            Schema = new OpenApiSchema { Type = "string" },
                            Description = "Category description"
                        });

                        operation.Parameters.Add(new OpenApiParameter
                        {
                            Name = "image",
                            In = ParameterLocation.Query,
                            Required = false,
                            Schema = new OpenApiSchema { Type = "string" },
                            Description = "Category image URL"
                        });
                    }

                    
                    if (parameter.ParameterType.Name == "CreateSubCategoryDTO")
                    {
                        operation.Parameters.Add(new OpenApiParameter
                        {
                            Name = "name",
                            In = ParameterLocation.Query,
                            Required = true,
                            Schema = new OpenApiSchema { Type = "string" },
                            Description = "Subcategory name"
                        });

                        operation.Parameters.Add(new OpenApiParameter
                        {
                            Name = "description",
                            In = ParameterLocation.Query,
                            Required = false,
                            Schema = new OpenApiSchema { Type = "string" },
                            Description = "Subcategory description"
                        });

                        operation.Parameters.Add(new OpenApiParameter
                        {
                            Name = "image",
                            In = ParameterLocation.Query,
                            Required = false,
                            Schema = new OpenApiSchema { Type = "string" },
                            Description = "Subcategory image URL"
                        });

                        operation.Parameters.Add(new OpenApiParameter
                        {
                            Name = "categoryId",
                            In = ParameterLocation.Query,
                            Required = true,
                            Schema = new OpenApiSchema { Type = "string" },
                            Description = "Parent category ID"
                        });
                    }

                    
                    if (parameter.ParameterType.Name == "UpdateSubCategoryDTO")
                    {
                        operation.Parameters.Add(new OpenApiParameter
                        {
                            Name = "id",
                            In = ParameterLocation.Query,
                            Required = true,
                            Schema = new OpenApiSchema { Type = "string" },
                            Description = "Subcategory ID"
                        });

                        operation.Parameters.Add(new OpenApiParameter
                        {
                            Name = "name",
                            In = ParameterLocation.Query,
                            Required = true,
                            Schema = new OpenApiSchema { Type = "string" },
                            Description = "Subcategory name"
                        });

                        operation.Parameters.Add(new OpenApiParameter
                        {
                            Name = "description",
                            In = ParameterLocation.Query,
                            Required = false,
                            Schema = new OpenApiSchema { Type = "string" },
                            Description = "Subcategory description"
                        });

                        operation.Parameters.Add(new OpenApiParameter
                        {
                            Name = "image",
                            In = ParameterLocation.Query,
                            Required = false,
                            Schema = new OpenApiSchema { Type = "string" },
                            Description = "Subcategory image URL"
                        });

                        operation.Parameters.Add(new OpenApiParameter
                        {
                            Name = "categoryId",
                            In = ParameterLocation.Query,
                            Required = true,
                            Schema = new OpenApiSchema { Type = "string" },
                            Description = "Parent category ID"
                        });
                    }
                }
            }
        }
    }
} 
