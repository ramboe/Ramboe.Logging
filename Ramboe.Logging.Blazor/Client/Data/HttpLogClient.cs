using System.Net.Http.Json;
using Ramboe.Logging.Blazor.Shared;

namespace Ramboe.Logging.Blazor.Client.Data;

public class HttpLogClient
{
    readonly HttpClient _client;

    public HttpLogClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<LogModel[]?> GetByWebservice(string servicename)
    {
        var logs = await _client.GetFromJsonAsync<LogModel[]>($"Log/GetByWebservice/{servicename}");

        return logs;
    }

    public async Task<DashBoardModel[]?> GetDashBoardDataForService(string service)
    {
        var logs = await _client.GetFromJsonAsync<DashBoardModel[]>($"Log/GetDashBoardDataForService/{service}");

        return logs;
    }

    public async Task<LogModel[]?> GetById(string id)
    {
        var logs = await _client.GetFromJsonAsync<LogModel[]>($"Log/GetById/{id}");

        return logs;
    }

    public async Task<LogModel[]?> GetByLevel(string level)
    {
        var logs = await _client.GetFromJsonAsync<LogModel[]>($"Log/GetByLevel/{level}");

        return logs;
    }

    public async Task<IEnumerable<string>> GetLogLevels()
    {
        var list = await _client.GetFromJsonAsync<IEnumerable<string>>("Log/GetLogLevels");

        return list ?? new List<string>();
    }

    public async Task<IEnumerable<string>> GetServices()
    {
        var list = await _client.GetFromJsonAsync<IEnumerable<string>>("Log/GetServices");

        return list ?? new List<string>();
    }

    public async Task<string> MarkAsDone(string id)
    {
        return await _client.GetStringAsync($"Log/MarkAsDone/{id}/");
    }

    public async Task<LogModel[]?> GetAll()
    {
        var logs = await _client.GetFromJsonAsync<LogModel[]>("Log");

        return logs;
    }
}
