using MediatR;
using Microsoft.AspNetCore.Mvc;
using RemoteExecutorGateWayApi.Commands;
using RemoteExecutorGateWayApi.Controllers;

namespace RemoteExecutorApi.Controllers;

[ApiController]
[Route("api/{**path}")] // The catch-all route
public class ExecutorController : ControllerBase
{
    private readonly IMediator mediator;
    private readonly ICommandFactory commandFactory;
    public ExecutorController(IMediator mediator, ICommandFactory commandFactory)
    {
        this.mediator = mediator;
        this.commandFactory = commandFactory;
    }

    [HttpPost(Name = "Run")]
    public async Task<ActionResult> Run([FromBody] ExecutorJsonRequest request)
    {
        var command = commandFactory.Create(request);
        var result = await mediator.Send(command);

        return Ok(result.Value);
    }
}

