using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RemoteExecutorGateWayApi.UnitTests.Helpers
{
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        public HttpResponseMessage Response { get; set; } = new(HttpStatusCode.OK);
        public Queue<HttpResponseMessage> ResponseQueue { get; set; } = new();
        public int CallCount { get; private set; }

        public void ResetCallCount() => CallCount = 0;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            CallCount++;
            if (ResponseQueue.TryDequeue(out var queuedResponse))
            {
                return queuedResponse;
            }
            return await Task.FromResult(Response);
        }
    }
}
