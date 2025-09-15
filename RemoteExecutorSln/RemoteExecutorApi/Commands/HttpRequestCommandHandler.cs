using Ardalis.Result;
using Ardalis.SharedKernel;
using FluentValidation;
using RemoteExecutorGateWayApi.Commands;
using RemoteExecutorGateWayApi.ViewModels.Responses;

namespace RemoteExecutorGateWayApi.Commands;

    public class HttpRequestCommandHandler(
    IValidator<HttpRequestCommand> _validator)
  : ICommandHandler<HttpRequestCommand, Result<ExecutorResponse>>
{
    public async Task<Result<ExecutorResponse>> Handle(HttpRequestCommand request,
      CancellationToken cancellationToken)
    {
        _validator.ValidateAndThrow(request);

        // call exeternal apis and get response

        return new Result<ExecutorResponse>(null);
    }
}
