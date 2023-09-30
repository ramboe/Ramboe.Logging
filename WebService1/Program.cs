using Ramboe.Logging.DependencyInjection;
using Serilog;
using Serilog.Events;
using WebService1.TypedHttpClients;

var builder = WebApplication.CreateBuilder(args);

#region ramboe logging
var connectionString = builder.Configuration.GetValue<string>("ConnectionStrings:Logging");

builder.Services.AddRamboeLogging(connectionString, LogEventLevel.Error);
builder.Host.UseSerilog();

builder.Services.AddTypedRamboeLoggingHttpClient<HttpWebService2Client>("https://localhost:7100");
#endregion

var app = builder.Build();

#region ramboe logging
app.UseRamboeExceptionLoggingMiddleware();
#endregion

app.MapGet("/", () => "Hello World!");
app.MapGet("/error/{id}", throwCustomErrorTest);
app.MapGet("/error-across-services/{id}", accros);

app.Run();

return;

void throwCustomErrorTest(string id)
{
    throw new Exception($"example error here {id}");
}

async Task accros(HttpWebService2Client client)
{
    await client.GetAsync();
}
