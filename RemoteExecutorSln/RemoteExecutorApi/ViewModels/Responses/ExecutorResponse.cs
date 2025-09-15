namespace RemoteExecutorGateWayApi.ViewModels.Responses;

public class ExecutorResponse
{
    public ExecutorResponse(
        Guid correlationId,
        string status,
        DateTime startTimeUtc,
        DateTime endTimeUtc,
        AttemptSummary attemptSummary,
        dynamic results)
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
    public dynamic Results { get; private set; }

}
