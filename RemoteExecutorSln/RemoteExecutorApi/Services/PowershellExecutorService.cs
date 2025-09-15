using RemoteExecutorGateWayApi.Controllers;
using RemoteExecutorGateWayApi.ViewModels.Requests;
using RemoteExecutorGateWayApi.ViewModels.Responses;

namespace RemoteExecutorGateWayApi.Services
{
    public class PowershellExecutorService : IPowershellExecutorService
    {
        public async Task<ExecutorResponse> ExecuteAsync(PowerShellExecutorRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }


        }
    }
}
