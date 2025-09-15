using RemoteExecutorGateWayApi.Controllers;
using RemoteExecutorGateWayApi.ViewModels.Responses;

namespace RemoteExecutorGateWayApi.Services
{
    public interface IOrchestratorService
    {
        Task<ExecutorResponse> ExecuteAsync(ExecutorJsonRequest executorJson);
    }
}