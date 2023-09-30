using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Ramboe.Logging.Blazor.Client.ComponentBaseExtensions;

public class TryWrapper : ComponentBase
{
    [Inject] public ISnackbar Snackbar { get; set; }

    public bool Loading { get; set; }

    // public string ErrorMessage { get; set; }

    [DebuggerHidden] protected void Try(Action methodCall)
    {
        Loading = true;

        try
        {
            methodCall();
        }
        catch (HttpRequestException exc)
        {
            Snackbar.Add(exc.Message, Severity.Error);
        }

        Loading = false;
        StateHasChanged();
    }

    [DebuggerHidden] protected async Task TryAsync(Func<Task> asyncCall)
    {
        Loading = true;
        await InvokeAsync(StateHasChanged);

        try
        {
            await asyncCall();
        }
        catch (HttpRequestException exc)
        {
            // ErrorMessage = exc.Message;
            Snackbar.Add(exc.Message, Severity.Error);
        }

        Loading = false;

        await InvokeAsync(StateHasChanged);
    }

    [DebuggerHidden] protected async Task<T> TryAsync<T>(Func<Task<T>> asyncCall) 
    {
        Loading = true;

        try
        {
            var result = await asyncCall();

            Loading = false;
            await InvokeAsync(StateHasChanged);

            return result;
        }
        catch (HttpRequestException ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);

            Loading = false;
            await InvokeAsync(StateHasChanged);

            return default;
        }
    }
}
