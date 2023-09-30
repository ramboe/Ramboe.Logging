using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using Ramboe.Logging.Blazor.Client;
using Ramboe.Logging.Blazor.Client.Auth;
using Ramboe.Logging.Blazor.Client.Data;
using Ramboe.Logging.Blazor.Client.HttpMessageHandlers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var env = builder.HostEnvironment.Environment;

var url = new Uri(builder.HostEnvironment.BaseAddress);

if (env == "Production")
{
    url = new Uri("https://logging.ramboe.de/");
}

builder.Services.AddScoped(sp => new HttpClient {BaseAddress = url});

builder.Services
       .AddHttpClient<HttpLoginClient>()
       .ConfigureHttpClient(c => c.BaseAddress = url)
       .AddHttpMessageHandler<ErrorDisplayMessageHandler>();

builder.Services.AddScoped<AuthHandler>();
builder.Services.AddScoped<ErrorDisplayMessageHandler>();

builder.Services
       .AddHttpClient<HttpLogClient>()
       .ConfigureHttpClient(c => c.BaseAddress = url)
       .AddHttpMessageHandler<AuthHandler>()
       .AddHttpMessageHandler<ErrorDisplayMessageHandler>();

builder.Services.AddAuthorizationCore();

builder.Services.AddMudServices(config => {
    config.SnackbarConfiguration.NewestOnTop = true;
    config.SnackbarConfiguration.MaxDisplayedSnackbars = 10;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.VisibleStateDuration = 5000;
    config.SnackbarConfiguration.HideTransitionDuration = 300;
    config.SnackbarConfiguration.ShowTransitionDuration = 300;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
    config.SnackbarConfiguration.ClearAfterNavigation = true;
}
);

builder.Services.AddBlazoredLocalStorage(config => config.JsonSerializerOptions.WriteIndented = true);
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
await builder.Build().RunAsync();
