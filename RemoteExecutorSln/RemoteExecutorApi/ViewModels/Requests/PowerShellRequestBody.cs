namespace RemoteExecutorGateWayApi.ViewModels.Requests;

public class PowerShellRequestBody
{
    public string Command { get; private set; }
    public Dictionary<string, string> Parameters { get; private set; }
}
