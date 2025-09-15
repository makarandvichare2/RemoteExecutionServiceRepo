using RemoteExecutorGateWayApi.Enums;

namespace RemoteExecutorGateWayApi.ViewModels.Requests;

public class HttpExecutorRequest : ExecutorRequest
{
    public HttpRequestBody RequestBody { get; private set; }
    public HttpExecutorRequest(HttpRequestBody body,
                               ExecutorRequestPolicy? executionPolicy) : base(executionPolicy)
    {
        RequestBody = body;
    }
}
