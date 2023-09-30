using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Ramboe.Logging.DependencyInjection.Middleware;

/// <summary>
/// This middleware is responsible for logging any exception thrown by your service. If plugged into the pipeline, any exception will be written into a PostgreSQL database of your choice, along with it's http context.
/// </summary>
public class RamboeExceptionHandlerMiddleware : IMiddleware
{
    readonly ILogger<RamboeExceptionHandlerMiddleware> _logger;

    string _serviceName = string.Empty;

    public RamboeExceptionHandlerMiddleware(ILogger<RamboeExceptionHandlerMiddleware> logger, string name)
    {
        _logger = logger;
        _serviceName = name;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            string traceIdentifier = context.Request.Headers["X-Trace-Id"];

            if (!string.IsNullOrEmpty(traceIdentifier))
            {
                context.TraceIdentifier = traceIdentifier;
            }

            var id = context.TraceIdentifier;
            var url = context.Request.Path.ToString();
            var verb = context.Request.Method;
            var queryParams = context.Request.QueryString.ToString();
            var body = string.Empty;

            if (context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Patch)
            {
                var httpBody = context.Request.Body;

                httpBody.Position = 0;

                body = await new System.IO.StreamReader(httpBody).ReadToEndAsync();
            }

            var format = extractLine(e);

            _logger.LogError("{service}, {message_short}, {http_path}, {id}, {http_verb},{http_body}, {http_queryparams}, {location_in_stacktrace}, {stack_trace}",
            _serviceName,
            e.Message,
            url,
            id,
            verb,
            body,
            queryParams,
            format,
            e.StackTrace);

            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var errorresult = new
            {
                Id = id,
                Path = url,
                ServiceName = _serviceName,
                ErrorMessage = e.Message,
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorresult));
        }
    }

    /// <summary>
    /// Use regular expressions to extract file name and line number from the most relevant source of the exception
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    static string extractLine(Exception e)
    {
        var match = Regex.Match(e.StackTrace, @"(\w+\.cs):\w+\s+(\d+)");

        if (!match.Success)
        {
            return e.Message;
        }

        try
        {
            var fileName = match.Groups[1].Value;
            var lineNumber = match.Groups[2].Value;

            return $"File: {fileName}, Line: {lineNumber}";
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);

            throw;
        }
    }
}
