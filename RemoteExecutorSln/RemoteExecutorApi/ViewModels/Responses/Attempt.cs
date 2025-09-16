namespace RemoteExecutorGateWayApi.ViewModels.Responses;

public class Attempt
{
    public int AttemptNumber { get; set; }
    public string Status { get; set; }
    public string ErrorMessage { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public DateTimeOffset StartTimeUtc { get; set; }
    public DateTimeOffset EndTimeUtc { get; set; }
}
