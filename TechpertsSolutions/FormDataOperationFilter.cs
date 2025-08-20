using Core.Enums;
using Core.Utilities;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

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
                if (
                    parameter
                        .GetCustomAttributes(
                            typeof(Microsoft.AspNetCore.Mvc.FromFormAttribute),
                            false
                        )
                        .Any()
                )
                {
                    if (operation.Parameters == null)
                        operation.Parameters = new List<OpenApiParameter>();

                    if (parameter.ParameterType.Name == "CategoryCreateDTO")
                    {
                        operation.Parameters.Add(
                            new OpenApiParameter
                            {
                                Name = "name",
                                In = ParameterLocation.Query,
                                Required = true,
                                Schema = new OpenApiSchema { Type = "string" },
                                Description = "Category name",
                            }
                        );

                        operation.Parameters.Add(
                            new OpenApiParameter
                            {
                                Name = "description",
                                In = ParameterLocation.Query,
                                Required = false,
                                Schema = new OpenApiSchema { Type = "string" },
                                Description = "Category description",
                            }
                        );

                        operation.Parameters.Add(
                            new OpenApiParameter
                            {
                                Name = "image",
                                In = ParameterLocation.Query,
                                Required = false,
                                Schema = new OpenApiSchema { Type = "string" },
                                Description = "Category image URL",
                            }
                        );
                    }

                    if (parameter.ParameterType.Name == "CategoryUpdateDTO")
                    {
                        operation.Parameters.Add(
                            new OpenApiParameter
                            {
                                Name = "id",
                                In = ParameterLocation.Query,
                                Required = true,
                                Schema = new OpenApiSchema { Type = "string" },
                                Description = "Category ID",
                            }
                        );

                        operation.Parameters.Add(
                            new OpenApiParameter
                            {
                                Name = "name",
                                In = ParameterLocation.Query,
                                Required = true,
                                Schema = new OpenApiSchema { Type = "string" },
                                Description = "Category name",
                            }
                        );

                        operation.Parameters.Add(
                            new OpenApiParameter
                            {
                                Name = "description",
                                In = ParameterLocation.Query,
                                Required = false,
                                Schema = new OpenApiSchema { Type = "string" },
                                Description = "Category description",
                            }
                        );

                        operation.Parameters.Add(
                            new OpenApiParameter
                            {
                                Name = "image",
                                In = ParameterLocation.Query,
                                Required = false,
                                Schema = new OpenApiSchema { Type = "string" },
                                Description = "Category image URL",
                            }
                        );
                    }

                    if (parameter.ParameterType.Name == "CreateSubCategoryDTO")
                    {
                        operation.Parameters.Add(
                            new OpenApiParameter
                            {
                                Name = "name",
                                In = ParameterLocation.Query,
                                Required = true,
                                Schema = new OpenApiSchema { Type = "string" },
                                Description = "Subcategory name",
                            }
                        );

                        operation.Parameters.Add(
                            new OpenApiParameter
                            {
                                Name = "description",
                                In = ParameterLocation.Query,
                                Required = false,
                                Schema = new OpenApiSchema { Type = "string" },
                                Description = "Subcategory description",
                            }
                        );

                        operation.Parameters.Add(
                            new OpenApiParameter
                            {
                                Name = "image",
                                In = ParameterLocation.Query,
                                Required = false,
                                Schema = new OpenApiSchema { Type = "string" },
                                Description = "Subcategory image URL",
                            }
                        );

                        operation.Parameters.Add(
                            new OpenApiParameter
                            {
                                Name = "categoryId",
                                In = ParameterLocation.Query,
                                Required = true,
                                Schema = new OpenApiSchema { Type = "string" },
                                Description = "Parent category ID",
                            }
                        );
                    }

                    if (parameter.ParameterType.Name == "UpdateSubCategoryDTO")
                    {
                        operation.Parameters.Add(
                            new OpenApiParameter
                            {
                                Name = "id",
                                In = ParameterLocation.Query,
                                Required = true,
                                Schema = new OpenApiSchema { Type = "string" },
                                Description = "Subcategory ID",
                            }
                        );

                        operation.Parameters.Add(
                            new OpenApiParameter
                            {
                                Name = "name",
                                In = ParameterLocation.Query,
                                Required = true,
                                Schema = new OpenApiSchema { Type = "string" },
                                Description = "Subcategory name",
                            }
                        );

                        operation.Parameters.Add(
                            new OpenApiParameter
                            {
                                Name = "description",
                                In = ParameterLocation.Query,
                                Required = false,
                                Schema = new OpenApiSchema { Type = "string" },
                                Description = "Subcategory description",
                            }
                        );

                        operation.Parameters.Add(
                            new OpenApiParameter
                            {
                                Name = "image",
                                In = ParameterLocation.Query,
                                Required = false,
                                Schema = new OpenApiSchema { Type = "string" },
                                Description = "Subcategory image URL",
                            }
                        );

                        operation.Parameters.Add(
                            new OpenApiParameter
                            {
                                Name = "categoryId",
                                In = ParameterLocation.Query,
                                Required = true,
                                Schema = new OpenApiSchema { Type = "string" },
                                Description = "Parent category ID",
                            }
                        );
                    }

                    if (parameter.ParameterType.Name == "RegisterDTO")
                    {
                        operation.RequestBody = new OpenApiRequestBody
                        {
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                ["multipart/form-data"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        Type = "object",
                                        Properties = new Dictionary<string, OpenApiSchema>
                                        {
                                            ["fullName"] = new OpenApiSchema
                                            {
                                                Type = "string",
                                                Description = "User's full name",
                                            },
                                            ["userName"] = new OpenApiSchema
                                            {
                                                Type = "string",
                                                Description = "Username",
                                            },
                                            ["email"] = new OpenApiSchema
                                            {
                                                Type = "string",
                                                Description = "Email address",
                                            },
                                            ["password"] = new OpenApiSchema
                                            {
                                                Type = "string",
                                                Description = "Password",
                                            },
                                            ["confirmPassword"] = new OpenApiSchema
                                            {
                                                Type = "string",
                                                Description = "Password confirmation",
                                            },
                                            ["address"] = new OpenApiSchema
                                            {
                                                Type = "string",
                                                Description = "User's address",
                                            },
                                            ["phoneNumber"] = new OpenApiSchema
                                            {
                                                Type = "string",
                                                Description = "Phone number",
                                            },
                                            ["city"] = new OpenApiSchema
                                            {
                                                Type = "string",
                                                Description = "City",
                                            },
                                            ["country"] = new OpenApiSchema
                                            {
                                                Type = "string",
                                                Description = "Country",
                                            },
                                            ["profilePhoto"] = new OpenApiSchema
                                            {
                                                Type = "string",
                                                Format = "binary",
                                                Description = "Profile photo file",
                                            },
                                            ["role"] = new OpenApiSchema
                                            {
                                                Type = "string",
                                                Description =
                                                    "User role (Customer, Admin, TechCompany, DeliveryPerson)",
                                            },
                                        },
                                        Required = new HashSet<string>
                                        {
                                            "fullName",
                                            "userName",
                                            "email",
                                            "password",
                                            "confirmPassword",
                                            "address",
                                            "phoneNumber",
                                            "role",
                                        },
                                    },
                                },
                            },
                        };
                    }
                }
            }
        }
    }
}
