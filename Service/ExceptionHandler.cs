using System.Text.Json;
using Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Service;

public class ExceptionHandler(ILogger<ExceptionHandler> logger) : IExceptionHandler
{
    private static JsonSerializerOptions SerializerOptions => new(JsonSerializerDefaults.Web);
    
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Title = "An error occurred",
            Detail = exception.Message,
            Status = 500,
            Extensions = new Dictionary<string, object?>
            {
                { "traceId", httpContext.TraceIdentifier }
            }
        };

        switch (exception)
        {
            case IException internalException:
                problemDetails.Status = internalException.StatusCode;
                logger.LogError(exception,
                    "Exception: was thrown for request with id: {traceId}", httpContext.TraceIdentifier);
                break;
            default:
                logger.LogError(exception,
                    "Exception: was thrown for request with id: {traceId}", httpContext.TraceIdentifier);
                break;
        }
        
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, SerializerOptions), cancellationToken);

        return true;
    }
}