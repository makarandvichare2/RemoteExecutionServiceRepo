namespace RemoteExecutorGateWayApi.ViewModels.Responses;

public class AttemptSummary
{
    public int AttemptCount { get; private set; }
    public Attempt Attempts { get; private set; }
}
