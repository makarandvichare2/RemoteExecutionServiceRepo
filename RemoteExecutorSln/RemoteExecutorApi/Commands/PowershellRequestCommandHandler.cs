using Ardalis.Result;
using Ardalis.SharedKernel;
using FluentValidation;
using RemoteExecutorGateWayApi.Commands;
using RemoteExecutorGateWayApi.ViewModels.Responses;

namespace RemoteExecutorGateWayApi.Commands;

    public class PowershellRequestCommandHandler(
    IValidator<PowershellRequestCommand> _validator)
  : ICommandHandler<PowershellRequestCommand, Result<ExecutorResponse>>
{
    public async Task<Result<ExecutorResponse>> Handle(PowershellRequestCommand request,
      CancellationToken cancellationToken)
    {
        _validator.ValidateAndThrow(request);

        // call exeternal apis and get response

        return new Result<ExecutorResponse>(null);
    }
}
