using System.Text.Json;
using Ramboe.Logging.Blazor.Shared;

namespace Ramboe.Logging.Blazor.Server.Middleware;

/// <summary>
/// necessary to display exceptions from the api properly in the frontend
/// </summary>
public class ErrorMessageMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            var errorMessage = new ErrorMessageModel
            {
                Id = Guid.NewGuid().ToString(),
                Message = e.Message
            };

            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            
            await context.Response.WriteAsync(JsonSerializer.Serialize(errorMessage));
        }
    }
}
