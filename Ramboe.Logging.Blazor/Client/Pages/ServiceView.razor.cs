using Microsoft.AspNetCore.Components;
using Ramboe.Logging.Blazor.Client.Data;
using Ramboe.Logging.Blazor.Shared;

namespace Ramboe.Logging.Blazor.Client.Pages;

public partial class ServiceView
{
    [Inject] public HttpLogClient HttpLogClient { get; set; }

    [Parameter] public string ServiceName { get; set; }

    private LogModel[] LogsFromApi { get; set; } =
    {
    };

    HashSet<LogModel> SelectedItems { get; set; } = new HashSet<LogModel>();

    private string _searchString;

    private bool _sortNameByLength;

    protected override async Task OnParametersSetAsync()
    {
        await Refresh();
    }

    async Task MarkAsDoneAndRefresh()
    {
        await TryAsync(execute);

        async Task execute()
        {
            foreach (var item in SelectedItems)
            {
                await HttpLogClient.MarkAsDone(item.Id);
            }

            LogsFromApi = await HttpLogClient.GetByWebservice(ServiceName);
        }
    }

    async Task Refresh()
    {
        await TryAsync(execute);

        async Task execute()
        {
            LogsFromApi = await HttpLogClient.GetByWebservice(ServiceName);
        }
    }

    private Func<LogModel, object> _sortBy => x => {
        if (_sortNameByLength)
            return x.Id.Length;
        else
            return x.Id;
    };

    // quick filter - filter globally across multiple columns with the same input
    private Func<LogModel, bool> _quickFilter => x => {
        if (string.IsNullOrWhiteSpace(_searchString))
            return true;

        if (x.Id.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
            return true;

        if (x.message_short.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
            return true;

        if ($"{x.level} {x.service} {x.http_path}".Contains(_searchString))
            return true;

        return false;
    };

    void UpdateSelectedItems(HashSet<LogModel> items)
    {
        SelectedItems = items;
    }
}
