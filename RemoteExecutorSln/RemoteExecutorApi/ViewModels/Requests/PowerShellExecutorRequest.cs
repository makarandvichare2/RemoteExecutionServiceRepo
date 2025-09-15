using RemoteExecutorGateWayApi.Enums;

namespace RemoteExecutorGateWayApi.ViewModels.Requests;

public class PowerShellExecutorRequest : ExecutorRequest
{
    public PowerShellRequestBody RequestBody { get; private set; }
    public PowerShellExecutorRequest(PowerShellRequestBody body,
                               ExecutorRequestPolicy? executionPolicy) : base(executionPolicy)
    {
        this.RequestBody = body;
    }
}
