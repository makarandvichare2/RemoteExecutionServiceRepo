namespace RemoteExecutorGateWayApi.ViewModels.Requests;

public class ExecutorRequestPolicy
{
    public int MaxRetries { get; set; }
    public int DelayTimeoutInSeconds { get; set; }
    public int MaxEventBeforeBreak { get; set; }
    public int BreakTimeoutInSeconds { get; set; }
}
