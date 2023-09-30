using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using MudBlazor;
using Ramboe.Logging.Blazor.Client.Data;
using Ramboe.Logging.Blazor.Shared;

namespace Ramboe.Logging.Blazor.Client.Pages;

public partial class Logs
{
    [Inject] public HttpLogClient HttpLogClient { get; set; }

    [Parameter] public string Level { get; set; }

    // [Parameter] public string Id { get; set; }

    private LogModel[] LogsFromApi { get; set; } =
    {
    };

    HashSet<LogModel> SelectedItems { get; set; } = new HashSet<LogModel>();

    public string BackgroundColorForDoubles { get; set; } = "#258cfb";

    protected override async Task OnParametersSetAsync()
    {
        await Refresh();
    }

    async Task Refresh()
    {
        async Task execute()
        {
            if (string.IsNullOrEmpty(Level))
            {
                LogsFromApi = await HttpLogClient.GetAll();
            }
            else
            {
                LogsFromApi = await HttpLogClient.GetByLevel(Level);
            }
        }

        await TryAsync(execute);
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

            LogsFromApi = await HttpLogClient.GetAll();
        }
    }

    ///////////
    ///
    private string _searchString;

    private bool _sortNameByLength;

    // private List<string> _events = new();

    // custom sort by name length
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

    // events
    void RowClicked(DataGridRowClickEventArgs<LogModel> args)
    {
        // _events.Insert(0, $"Event = RowClick, Index = {args.RowIndex}, Data = {System.Text.Json.JsonSerializer.Serialize(args.Item)}");
    }

    void UpdateSelectedItems(HashSet<LogModel> items)
    {
        SelectedItems = items;
    }

    public Dictionary<string, string> Colors { get; set; } = new();

    private Func<LogModel, int, string> _rowStyleFunc => (x, i) => {
        if (LogsFromApi.Count(log => log.Id == x.Id) <= 1)
        {
            return "";
        }

        double percentage = 15;

        if (Colors.ContainsKey(x.Id))
        {
            BackgroundColorForDoubles = Colors[x.Id];
        }
        else
        {
            // BackgroundColorForDoubles = ColorHelper.DarkenHexColor(BackgroundColorForDoubles, percentage);
            BackgroundColorForDoubles = ColorHelper.GenerateRandomColor("#258cfb");
            Colors.Add(x.Id, BackgroundColorForDoubles);
        }

        return $"background-color: {BackgroundColorForDoubles}";
    };
}
