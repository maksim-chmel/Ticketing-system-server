using Microsoft.AspNetCore.Mvc;
using AdminPanelBack.Exceptions;

namespace AdminPanelBack.Middleware;

public class ErrorHandlingMiddleware(
    RequestDelegate next,
    ILogger<ErrorHandlingMiddleware> logger,
    IHostEnvironment env)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception for {Method} {Path}. TraceId={TraceId}",
                context.Request.Method,
                context.Request.Path.Value,
                context.TraceIdentifier);

            context.Response.StatusCode = ex switch
            {
                HttpException httpEx => httpEx.StatusCode,
                _ => StatusCodes.Status500InternalServerError
            };

            var title = ex is HttpException httpExForTitle
                ? httpExForTitle.Title
                : "Internal Server Error";

            var detail = ex switch
            {
                HttpException httpExForDetail => httpExForDetail.Detail,
                _ => env.IsDevelopment() ? ex.Message : "An unexpected error occurred."
            };

            var response = new ProblemDetails
            {
                Status = context.Response.StatusCode,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path
            };

            response.Extensions["traceId"] = context.TraceIdentifier;

            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}