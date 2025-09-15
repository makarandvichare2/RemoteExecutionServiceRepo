using RemoteExecutorGateWayApi.ViewModels.Requests;
using RemoteExecutorGateWayApi.ViewModels.Responses;

namespace RemoteExecutorGateWayApi.Services
{
    public interface IHttpExecutorService
    {
        Task<ExecutorResponse> ExecuteAsync(HttpExecutorRequest request);
    }
}