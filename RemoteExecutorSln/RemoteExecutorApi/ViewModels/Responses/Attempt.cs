namespace RemoteExecutorGateWayApi.ViewModels.Responses;

public class Attempt
{
    public Attempt(int attemptNumber, string status, DateTime startTimeUtc, DateTime endTimeUtc)
    {
        AttemptNumber = attemptNumber;
        Status = status;
        StartTimeUtc = startTimeUtc;
        EndTimeUtc = endTimeUtc;
    }

    public int AttemptNumber { get; private set; }
    public string Status { get; private set; }
    public DateTime StartTimeUtc { get; private set; }
    public DateTime EndTimeUtc { get; private set; }
}
