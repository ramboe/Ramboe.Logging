using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NpgsqlTypes;
using Ramboe.Logging.DependencyInjection.HttpMessageHandlers;
using Ramboe.Logging.DependencyInjection.Middleware;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;

namespace Ramboe.Logging.DependencyInjection;

public static class StartupExtensions
{
    /// <summary>
    /// Adds the following to the DI container
    /// - Middleware to handle and log exceptions that come up in the current web service
    /// - Message Handler for typed http clients to ensure consistent tracing IDs for message tracking across multiple web services
    /// - Serilog Logger that writes into the "logs" table of a postgresql database, specified by a given connection string. 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="connectionString"></param>
    /// <param name="logEventLevel"></param>
    /// <param name="serviceName">optional, will take DllName if not provided</param>
    /// <param name="enableSelfLoggingIntoConsole">provide 'true' if there is a need to see the serilog logs (helpful for debuggign if logs don't get written properly)</param>
    /// <returns></returns>
    public static IServiceCollection AddRamboeLogging(this IServiceCollection services, string connectionString, LogEventLevel logEventLevel, string serviceName = null,
        bool enableSelfLoggingIntoConsole = false)
    {
        if (string.IsNullOrEmpty(serviceName))
        {
            serviceName = getServiceNameFromDll();
        }

        //Middleware to handle and log exceptions that come up in the current web service
        services.AddTransient<RamboeExceptionHandlerMiddleware>(provider =>
            new RamboeExceptionHandlerMiddleware(provider.GetService<ILogger<RamboeExceptionHandlerMiddleware>>(), serviceName));

        //Message Handler for typed http clients to ensure consistent tracing IDs for message tracking across multiple web services
        services.AddHttpContextAccessor();
        services.AddScoped<RamboeTraceIdentifierHandler>();

        //for debugging purposes
        if (enableSelfLoggingIntoConsole)
        {
            Serilog.Debugging.SelfLog.Enable(Console.Error);
        }

        //Serilog Logger that writes into the "logs" table of a postgresql database, specified by a given connection string. 
        addPostgreSqlSerilogger(connectionString, logEventLevel);

        return services;
    }

    static void addPostgreSqlSerilogger(string connectionString, LogEventLevel logEventLevel)
    {
        IDictionary<string, ColumnWriterBase> columnWriters = new Dictionary<string, ColumnWriterBase>
        {
            {"id", new SinglePropertyColumnWriter("id", PropertyWriteMethod.ToString, NpgsqlDbType.Text, "l")},
            {"level", new LevelColumnWriter(true, NpgsqlDbType.Varchar)},
            {"raise_date", new TimestampColumnWriter(NpgsqlDbType.Timestamp)},
            {"service", new SinglePropertyColumnWriter("service", PropertyWriteMethod.ToString, NpgsqlDbType.Text, "l")},
            {"http_path", new SinglePropertyColumnWriter("http_path", PropertyWriteMethod.ToString, NpgsqlDbType.Text, "l")},
            {"http_verb", new SinglePropertyColumnWriter("http_verb", PropertyWriteMethod.ToString, NpgsqlDbType.Text, "l")},
            {"http_body", new SinglePropertyColumnWriter("http_body", PropertyWriteMethod.ToString, NpgsqlDbType.Text, "l")},
            {"http_queryparams", new SinglePropertyColumnWriter("http_queryparams", PropertyWriteMethod.ToString, NpgsqlDbType.Text, "l")},
            {"message_short", new SinglePropertyColumnWriter("message_short", PropertyWriteMethod.ToString, NpgsqlDbType.Text, "l")},
            {"location_in_stacktrace", new SinglePropertyColumnWriter("location_in_stacktrace", PropertyWriteMethod.ToString, NpgsqlDbType.Text, "l")},
            {"stack_trace", new SinglePropertyColumnWriter("stack_trace", PropertyWriteMethod.ToString, NpgsqlDbType.Text, "l")}
        };

        Log.Logger = new LoggerConfiguration()
                     .MinimumLevel.Is(logEventLevel)
                     .WriteTo.PostgreSQL(connectionString, "logs", needAutoCreateTable: true, columnOptions: columnWriters)
                     .CreateLogger();
    }

    public static IHttpClientBuilder AddTypedRamboeLoggingHttpClient<T>(this IServiceCollection services, string baseAddress) where T : class
    {
        return services.AddHttpClient<T>()
                       .AddHttpMessageHandler<RamboeTraceIdentifierHandler>()
                       .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseAddress));
    }

    public static IApplicationBuilder UseRamboeExceptionLoggingMiddleware(this IApplicationBuilder app)
    {
        app.Use((context, next) => {
            context.Request.EnableBuffering();

            return next();
        });

        return app.UseMiddleware<RamboeExceptionHandlerMiddleware>();
    }

    static string getServiceNameFromDll()
    {
        // Get the assembly information for the entry assembly (which is typically the API project)
        var assembly = Assembly.GetEntryAssembly();

        // Get the assembly name
        var assemblyName = assembly.GetName();

        return assemblyName.Name;
    }
}
