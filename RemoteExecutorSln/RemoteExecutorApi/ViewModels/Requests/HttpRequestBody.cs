namespace RemoteExecutorGateWayApi.ViewModels.Requests;

public class HttpRequestBody
{
    public string Method { get; set; }
    public string Url { get; set; }
    public Dictionary<string, string> Headers { get;  set; }
    public Dictionary<string, string> Body { get;  set; }
    public Dictionary<string, string> QueryParams { get;  set; }
}
