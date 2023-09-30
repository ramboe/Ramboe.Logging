using Microsoft.AspNetCore.Components;
using MudBlazor;
using Ramboe.Logging.Blazor.Client.Data;

namespace Ramboe.Logging.Blazor.Client.Shared;

public partial class NavMenu
{
    [Inject] public HttpLogClient HttpLogClient { get; set; }
    [Inject] public ISnackbar Snackbar { get; set; }

    public IEnumerable<string> WebServices { get; set; } = new List<string>();

    public IEnumerable<string> LogLevels { get; set; } = new List<string>();

    protected override async Task OnInitializedAsync()
    {
        await TryAsync(LoadLevelsAndServices);

        return;
    }

    async Task LoadLevelsAndServices()
    {
        LogLevels = await HttpLogClient.GetLogLevels();
        WebServices = await HttpLogClient.GetServices();
    }
}
