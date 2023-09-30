using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ramboe.Logging.Blazor.Server.Data;
using Ramboe.Logging.Blazor.Shared;

namespace Ramboe.Logging.Blazor.Server.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class LogController : ControllerBase
{
    readonly ILogRepository _repository;

    public LogController(ILogRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("GetByWebservice/{servicename}")] public async Task<IEnumerable<LogModel>> GetByWebservice(string servicename)
    {
        return await _repository.GetByWebservice(servicename);
    }

    [HttpGet("GetLogLevels")] public async Task<IEnumerable<string>> GetLogLevels()
    {
        return await _repository.GetLogLevels();
    }

    [HttpGet("GetServices")] public async Task<IEnumerable<string>> GetServices()
    {
        return await _repository.GetServices();
    }

    [HttpGet("GetDashBoardDataForService/{level}")] public async Task<IEnumerable<DashBoardModel>> GetDashBoardDataForService(string level)
    {
        return await _repository.GetDashBoardDataForService(level);
    }

    [HttpGet("GetByLevel/{level}")] public async Task<IEnumerable<LogModel>> GetByLevel(string level)
    {
        return await _repository.GetByLevel(level);
    }

    [HttpGet("MarkAsDone/{id}")] public async Task<string> MarkAsDone(string id)
    {
        await _repository.MarkAsDone(id);

        return id;
    }

    [HttpGet("GetById/{id}")] public async Task<IEnumerable<LogModel>> GetById(string id)
    {
        var models = await _repository.GetById(id);

        //CKRUSE Thursday, 28 September 2023: adjust HttpClients to display this correctly
        if (models is null || !models.Any())
        {
            throw new Exception("couldn't find any records with id" + id);
        }

        return models;
    }

    [HttpGet] public async Task<IEnumerable<LogModel>> Get()
    {
        return await _repository.GetAll();
    }
}
