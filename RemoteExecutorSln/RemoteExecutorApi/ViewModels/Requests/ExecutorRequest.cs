namespace RemoteExecutorGateWayApi.ViewModels.Requests;

public class ExecutorRequest
{
    public Guid CorrelationId { get; private set; }
    public ExecutorRequest(ExecutorRequestPolicy? executionPolicy)
    {
        CorrelationId = Guid.NewGuid();
        ExecutionPolicy = executionPolicy ?? defaultPolicy();
    }

    public ExecutorRequestPolicy ExecutionPolicy { get; private set; }

    private ExecutorRequestPolicy defaultPolicy()
    {
        return new ExecutorRequestPolicy
        {
            MaxRetries = 3,
            BreakTimeoutInSeconds = 15,
            MaxEventBeforeBreak = 5,
            DelayTimeoutInSeconds = 7,
        };
    }

}
