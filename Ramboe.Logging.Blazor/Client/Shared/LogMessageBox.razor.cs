using Microsoft.AspNetCore.Components;
using MudBlazor;
using Ramboe.Logging.Blazor.Client.Data;
using Ramboe.Logging.Blazor.Shared;

namespace Ramboe.Logging.Blazor.Client.Shared;

public partial class LogMessageBox
{
    [Inject] public NavigationManager NavigationManager { get; set; }

    [Inject] public HttpLogClient HttpLogClient { get; set; }

    [Parameter] public LogModel LogModel { get; set; }

    MudMessageBox mbox { get; set; }

    // string state = "Message box hasn't been opened yet";

    LogModel[]? AllLogsWithThisId { get; set; } = {};

    private async Task OnButtonClicked()
    {
        
        AllLogsWithThisId = await TryAsync(() => HttpLogClient.GetById(LogModel.Id));
        // await TryAsync(execute);

        var result = await mbox.Show(new DialogOptions
        {
            Position = DialogPosition.Center,
            MaxWidth = MaxWidth.ExtraLarge,

            // DisableBackdropClick = null,
            // CloseOnEscapeKey = true,
            // NoHeader = null,
            // CloseButton = null,
            // FullScreen = null,
            // FullWidth = null,
            // ClassBackground = null
        });
        // await TryAsync(execute);

        // await InvokeAsync(StateHasChanged);
        // AllLogsWithThisId = await TryAsync(() => HttpLogClient.GetById(LogModel.Id));

        // await InvokeAsync(StateHasChanged);

        // StateHasChanged();

        async Task execute()
        {
            AllLogsWithThisId = await HttpLogClient.GetById(LogModel.Id);
        }
    }

    void NavigateToWebserviceView()
    {
        NavigationManager.NavigateTo($"/Logs/Views/Service/{LogModel.service}");
    }
}
