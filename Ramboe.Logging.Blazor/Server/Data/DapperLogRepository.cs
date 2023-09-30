using Dapper;
using Npgsql;
using Ramboe.Logging.Blazor.Shared;

namespace Ramboe.Logging.Blazor.Server.Data;

/// <summary>
/// https://www.code4it.dev/blog/postgres-crud-dapper/
/// </summary>
public class DapperLogRepository : ILogRepository, IDisposable
{
    private NpgsqlConnection _connection;

    public DapperLogRepository(NpgsqlConnection connection)
    {
        _connection = connection;

        _connection.Open();
    }

    #region sortById
    public async Task<IEnumerable<LogModel>> GetByLevel(string level)
    {
        var commandText = $"SELECT * FROM logs WHERE lower(level) = lower('{level}') order by id, raise_date desc";

        var logs = await _connection.QueryAsync<LogModel>(commandText);

        return logs;
    }

    public async Task<IEnumerable<LogModel>> GetAll()
    {
        var commandText = "SELECT * FROM logs order by id, raise_date desc";

        try
        {
            var logs = await _connection.QueryAsync<LogModel>(commandText);

            return logs;
        }
        catch (PostgresException e)
        {
            if (e.Code != "42P01")// 42P01 = table not existing
            {
                throw;
            }

            await createIfNotExisting();

            return Array.Empty<LogModel>();
        }
    }

    public async Task<IEnumerable<LogModel>> GetByWebservice(string servicename)
    {
        var commandText = $"SELECT * FROM logs WHERE lower(service) = lower('{servicename}') order by id, raise_date desc";

        var logs = await _connection.QueryAsync<LogModel>(commandText);

        return logs;
    }
    #endregion

    public async Task<IEnumerable<LogModel>> GetById(string id)
    {
        var commandText = $"SELECT * FROM logs WHERE Id = '{id}'";

        var logs = await _connection.QueryAsync<LogModel>(commandText);

        return logs;
    }

    public async Task<IEnumerable<DashBoardModel>> GetDashBoardDataForService(string id)
    {
        var commandText = $"SELECT service, level, COUNT(*) as level_count FROM logs WHERE service = '{id}' GROUP BY service, level;";

        var logs = await _connection.QueryAsync<DashBoardModel>(commandText);

        return logs;
    }

    public async Task MarkAsDone(string id)
    {
        var commandText = "UPDATE logs SET level = @level WHERE id = @id";

        var queryArgs = new
        {
            id,
            level = "Done",
        };

        await _connection.ExecuteAsync(commandText, queryArgs);
    }

    public async Task<IEnumerable<string>> GetServices()
    {
        var commandText = "SELECT DISTINCT service FROM logs order by service";

        var services = await _connection.QueryAsync<string>(commandText);

        return services;
    }

    public async Task<IEnumerable<string>> GetLogLevels()
    {
        var commandText = "SELECT DISTINCT level FROM logs order by level";

        var services = await _connection.QueryAsync<string>(commandText);

        return services;
    }

    /// <summary>
    /// execute create table script
    /// </summary>
    /// <exception cref="FileNotFoundException"></exception>
    async Task createIfNotExisting()
    {
        var filePath = readSqlCommandFromFile("sql/createTableIfNotExisting.sql");

        var createTableCommand = await File.ReadAllTextAsync(filePath);

        var logs = await _connection.QueryAsync(createTableCommand);
    }

    string readSqlCommandFromFile(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"missing file '{path}'");
        }

        return path;
    }

    public void Dispose()
    {
        _connection.Close();
    }
}
