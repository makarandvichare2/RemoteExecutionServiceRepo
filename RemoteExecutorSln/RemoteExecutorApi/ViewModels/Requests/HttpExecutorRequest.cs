namespace RemoteExecutorGateWayApi.ViewModels.Requests;

public class HttpExecutorRequest : ExecutorRequest
{
    public HttpRequestBody RequestBody { get; private set; }
    public HttpExecutorRequest(HttpRequestBody body,
                               ExecutorRequestPolicy? executionPolicy) : base(executionPolicy)
    {
        //remove unwanted filters
        if (body.Headers != null && body.Headers.Count > 0)
        {
            body.Headers.Remove("Authorization");
            body.Headers.Remove("Connection");
            body.Headers.Remove("Keep-Alive");
            body.Headers.Remove("Cookie");
            body.Headers.Remove("Referer");
        }
        RequestBody = body;
    }
}
