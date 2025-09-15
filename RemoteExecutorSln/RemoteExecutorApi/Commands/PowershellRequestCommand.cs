using Ardalis.Result;
using Ardalis.SharedKernel;
using RemoteExecutorGateWayApi.ViewModels.Requests;
using RemoteExecutorGateWayApi.ViewModels.Responses;

namespace RemoteExecutorGateWayApi.Commands;
public record PowershellRequestCommand(PowerShellExecutorRequest CommandRequest) : ICommand<Result<ExecutorResponse>>;

