namespace RemoteExecutorGateWayApi.ViewModels.Responses;

public class ExecutorResponse
{
    public Guid CorrelationId { get; set; }
    public string Status { get; set; }
    public dynamic Result { get; set; }
    public AttemptSummary AttemptSummary { get; set; } = new();
}
