// File: ImportWizard.WebApi/Filters/FormFileOperationFilter.cs

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using ImportWizard.WebApi.Controllers;    // for ValidateRowsRequest

namespace ImportWizard.WebApi.Filters
{
    public class FormFileOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Only apply to the validateRows endpoint (or any with our ValidateRowsRequest)
            var hasBindModel = context.MethodInfo.GetParameters()
                .Any(p => p.ParameterType == typeof(ValidateRowsRequest));

            if (!hasBindModel)
                return;

            // Overwrite the RequestBody to declare exactly 2 fields
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
                                ["file"] = new OpenApiSchema
                                {
                                    Type = "string",
                                    Format = "binary",
                                    Description = "Excel file to validate"
                                },
                                ["mappings"] = new OpenApiSchema
                                {
                                    Type = "string",
                                    Description = "JSON object mapping column headers to DTO properties"
                                }
                            },
                            Required = new HashSet<string> { "file", "mappings" }
                        }
                    }
                }
            };
        }
    }
}
