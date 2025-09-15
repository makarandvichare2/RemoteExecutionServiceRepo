namespace RemoteExecutorGateWayApi.ViewModels.Requests;

public class ExecutorRequestPolicy
{
    public int MaxRetries { get; set; }
    public int TimeoutInSeconds { get; set; }
}
