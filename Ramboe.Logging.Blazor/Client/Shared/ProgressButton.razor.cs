using Microsoft.AspNetCore.Components;

namespace Ramboe.Logging.Blazor.Client.Shared;

public partial class ProgressButton
{
    [Parameter] public bool Loading { get; set; }
    [Parameter] public EventCallback Clicked { get; set; }
    [Parameter] public string Text { get; set; }
}
