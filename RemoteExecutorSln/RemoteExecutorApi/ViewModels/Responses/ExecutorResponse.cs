namespace RemoteExecutorGateWayApi.ViewModels.Responses;

public class ExecutorResponse
{
    ExecutorResponse(
        Guid correlationId,
        string status,
        DateTime startTimeUtc,
        DateTime endTimeUtc,
        AttemptSummary attemptSummary,
        string results)
    {
        CorrelationId = correlationId;
        Status = status;
        StartTimeUtc = startTimeUtc;
        EndTimeUtc = endTimeUtc;
        AttemptSummary = attemptSummary;
        Results = results;
    }

    public Guid CorrelationId { get; private set; }
    public string Status { get; private set; }
    public DateTime StartTimeUtc { get; private set; }
    public DateTime EndTimeUtc { get; private set; }
    public AttemptSummary AttemptSummary { get; private set; }
    public string Results { get; private set; }

}
