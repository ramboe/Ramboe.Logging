using Ramboe.Logging.Blazor.Shared;

namespace Ramboe.Logging.Blazor.Server.Data;

public interface ILogRepository
{
    Task<IEnumerable<LogModel>> GetAll();

    Task<IEnumerable<LogModel>> GetById(string id);

    Task MarkAsDone(string id);

    Task<IEnumerable<LogModel>> GetByLevel(string level);

    Task<IEnumerable<LogModel>> GetByWebservice(string servicename);

    Task<IEnumerable<string>> GetServices();

    Task<IEnumerable<string>> GetLogLevels();

    Task<IEnumerable<DashBoardModel>> GetDashBoardDataForService(string serviceName);
}
