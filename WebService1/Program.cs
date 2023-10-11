using System.Net.Http.Headers;
using Ramboe.Logging.DependencyInjection;
using Serilog;
using Serilog.Events;
using WebService1.TypedHttpClients;

var builder = WebApplication.CreateBuilder(args);

#region ramboe logging
var connectionString = builder.Configuration.GetValue<string>("ConnectionStrings:Logging");

//Make this service log errors into postgres
builder.Services.AddRamboeLogging(connectionString, LogEventLevel.Error);
builder.Host.UseSerilog();

//Add tracable http header to outgoing http requests to track errors across multiple services
const string _BASEURL = "https://localhost:7100";

const string _CLIENTNAME = "clemensClient";

builder.Services
       .AddRamboeLoggingHttpClient(_BASEURL, _CLIENTNAME)
       .ConfigureHttpClient(c => c.DefaultRequestHeaders
                                  .Accept
                                  .Add(new MediaTypeWithQualityHeaderValue("application/json")
                                  ));

builder.Services
       .AddRamboeLoggingHttpClient<HttpWebService2Client>(_BASEURL);
#endregion

var app = builder.Build();

#region ramboe logging
app.UseRamboeExceptionLoggingMiddleware();
#endregion

app.MapGet("/", () => "Hello World!");
app.MapGet("/error/{id}", throwCustomErrorTest);
app.MapGet("/error-across-services/{id}", accrosNamed);

app.Run();

return;

void throwCustomErrorTest(string id)
{
    throw new Exception($"example error here {id}");
}

async Task accrosNamed(IHttpClientFactory httpClientFactory)
{
    var client = httpClientFactory.CreateClient(_CLIENTNAME);
    var result = await client.GetStringAsync("");
}

async Task accrosTyped(HttpWebService2Client client)
{
    await client.GetAsync();
}
