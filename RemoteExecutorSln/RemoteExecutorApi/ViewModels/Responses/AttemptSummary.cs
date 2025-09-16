namespace RemoteExecutorGateWayApi.ViewModels.Responses;

public class AttemptSummary
{
    public List<Attempt> Attempts { get; set; } = new();
    public void AddAttempt(string status, string message = null) =>
        Attempts.Add(new Attempt { Status = status, ErrorMessage = message, Timestamp = DateTimeOffset.UtcNow });
}
