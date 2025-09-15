using RemoteExecutorGateWayApi.ViewModels.Requests;
using RemoteExecutorGateWayApi.ViewModels.Responses;

namespace RemoteExecutorGateWayApi.Services
{
    public interface IPowershellExecutorService
    {
        Task<ExecutorResponse> ExecuteAsync(PowerShellExecutorRequest request);
    }
}