namespace RemoteExecutorGateWayApi.ViewModels.Responses;

public class Attempt
{
    public int AttemptNumber { get; private set; }
    public string Status { get; private set; }
    public DateTime StartTimeUtc { get; private set; }
    public DateTime EndTimeUtc { get; private set; }
}
