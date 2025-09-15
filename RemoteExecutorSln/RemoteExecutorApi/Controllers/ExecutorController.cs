using Microsoft.AspNetCore.Mvc;
using RemoteExecutorGateWayApi.Controllers;
using RemoteExecutorGateWayApi.Services;

namespace RemoteExecutorApi.Controllers;

[ApiController]
[Route("api/{**path}")] // The catch-all route
public class ExecutorController : ControllerBase
{
    private readonly IOrchestratorService service;
    public ExecutorController(IOrchestratorService service)
    {
        this.service = service;
    }

    [HttpPost(Name = "Run")]
    public async Task<ActionResult> Run([FromBody] ExecutorJsonRequest request)
    {
        var result = await service.ExecuteAsync(request);

        return Ok(result);
    }
}

