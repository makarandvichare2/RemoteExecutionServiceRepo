using RemoteExecutorGateWayApi.Enums;

namespace RemoteExecutorGateWayApi.ViewModels.Requests;

public class ExecutorRequest
{
    public Guid CorrelationId { get; private set; }
    //public ExecutionTypeEnum ExecutionType { get; private set; }
    public ExecutorRequest(ExecutorRequestPolicy? executionPolicy)
    {
        CorrelationId = Guid.NewGuid();
        //ExecutionType = executionType;
        ExecutionPolicy = executionPolicy ?? defaultPolicy();
    }

    public ExecutorRequestPolicy ExecutionPolicy { get; private set; }

    private ExecutorRequestPolicy defaultPolicy()
    {
        return new ExecutorRequestPolicy
        {
            MaxRetries = 3,
            TimeoutInSeconds = 15,
        };
    }

}
