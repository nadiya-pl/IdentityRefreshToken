using ProtectedAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ProtectedAPI.Middlewares;


public class ErrorHandleMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandleMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ErrorHandleMiddleware(RequestDelegate next, ILogger<ErrorHandleMiddleware> logger, IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch(Exception ex)
        {
            if (httpContext.Response.HasStarted)
                throw;

            _logger.LogError(ex, "Unhandled exception {TraceId}", httpContext.TraceIdentifier);

            var (status, title) = MapException(ex);

            httpContext.Response.Clear();
            httpContext.Response.StatusCode = status;
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.Headers["X-Trace-Id"] = httpContext.TraceIdentifier;

            // если это девелопмент, то отдать все детали ошибки
            var message = _environment.IsDevelopment() ? $"{title}: {ex.Message}" : title;

            var response = new ProtectedResponse();
            response.Success = false;
            response.Message = message;
            response.TraceId = httpContext.TraceIdentifier;
            await httpContext.Response.WriteAsJsonAsync(response);
        }

    }

    private (int StatusCode, string Title) MapException(Exception ex)
    {
        if (ex is KeyNotFoundException)
        {
            return (StatusCodes.Status404NotFound, "Resource not found");
        }
        else if (ex is ArgumentException)
        {
            return (StatusCodes.Status400BadRequest, "Bad request");
        }
        else if (ex is UnauthorizedAccessException)
        {
            return (StatusCodes.Status401Unauthorized, "Unauthorized");
        }
        else
        {
            return (StatusCodes.Status500InternalServerError, "Unexpected error");
        }
    }
}


public static class ErrorHandleMiddlewareExtensions
{
    public static IApplicationBuilder UseErrorHandleMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandleMiddleware>();
    }
}
