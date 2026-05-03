using AdminPanelBack.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AdminPanelBack.Swagger;

public class SecurityRequirementsFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var declaringType = context.MethodInfo.DeclaringType;

        var hasJwtAuth = declaringType?.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() == true
                         || context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

        var hasApiKey = declaringType?.GetCustomAttributes(true).OfType<ServiceFilterAttribute>()
                            .Any(a => a.ServiceType == typeof(ApiKeyAuthFilter)) == true
                        || context.MethodInfo.GetCustomAttributes(true).OfType<ServiceFilterAttribute>()
                            .Any(a => a.ServiceType == typeof(ApiKeyAuthFilter));

        if (hasJwtAuth)
        {
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });
        }

        if (hasApiKey)
        {
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" }
                    },
                    Array.Empty<string>()
                }
            });
        }
    }
}
