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
                    // Remove the default request body for form data
                    operation.RequestBody = null;

                    // Add form parameters
                    if (operation.Parameters == null)
                        operation.Parameters = new List<OpenApiParameter>();

                    // Handle CategoryCreateDTO
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

                    // Handle CategoryUpdateDTO
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

                    // Handle CreateSubCategoryDTO
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

                    // Handle UpdateSubCategoryDTO
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

                    // Handle CartItemDTO
                    if (parameter.ParameterType.Name == "CartItemDTO")
                    {
                        operation.Parameters.Add(new OpenApiParameter
                        {
                            Name = "productId",
                            In = ParameterLocation.Query,
                            Required = true,
                            Schema = new OpenApiSchema { Type = "string" },
                            Description = "Product ID to add to cart"
                        });

                        operation.Parameters.Add(new OpenApiParameter
                        {
                            Name = "quantity",
                            In = ParameterLocation.Query,
                            Required = true,
                            Schema = new OpenApiSchema { Type = "integer" },
                            Description = "Quantity of the product"
                        });
                    }

                    // Handle CartUpdateItemQuantityDTO
                    if (parameter.ParameterType.Name == "CartUpdateItemQuantityDTO")
                    {
                        operation.Parameters.Add(new OpenApiParameter
                        {
                            Name = "productId",
                            In = ParameterLocation.Query,
                            Required = true,
                            Schema = new OpenApiSchema { Type = "string" },
                            Description = "Product ID to update"
                        });

                        operation.Parameters.Add(new OpenApiParameter
                        {
                            Name = "quantity",
                            In = ParameterLocation.Query,
                            Required = true,
                            Schema = new OpenApiSchema { Type = "integer" },
                            Description = "New quantity of the product"
                        });
                    }
                }
            }
        }
    }
} 