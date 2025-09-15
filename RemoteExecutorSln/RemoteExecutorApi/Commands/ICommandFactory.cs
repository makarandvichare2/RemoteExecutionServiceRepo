using Ardalis.Result;
using Ardalis.SharedKernel;
using RemoteExecutorGateWayApi.Controllers;
using RemoteExecutorGateWayApi.ViewModels.Responses;

namespace RemoteExecutorGateWayApi.Commands
{
    public interface ICommandFactory
    {
        HttpRequestCommand Create(ExecutorJsonRequest executorJson);
    }
}