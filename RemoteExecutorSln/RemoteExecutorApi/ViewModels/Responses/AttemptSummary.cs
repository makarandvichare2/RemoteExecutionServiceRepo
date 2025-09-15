namespace RemoteExecutorGateWayApi.ViewModels.Responses;

public class AttemptSummary
{
    public AttemptSummary()
    {
        Attempts = new List<Attempt>();
    }
    public int AttemptCount { get; set; }
    public List<Attempt> Attempts { get; set; }
}
